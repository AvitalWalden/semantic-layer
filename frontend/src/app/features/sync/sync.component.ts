import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ApiService } from '../../core/api.service';
import { SyncResultDto, SyncRunDto } from '../../core/models';

@Component({
  selector: 'app-sync',
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatProgressBarModule,
  ],
  templateUrl: './sync.component.html',
  styleUrl: './sync.component.scss',
})
export class SyncComponent implements OnInit {
  private readonly api = inject(ApiService);
  private readonly snack = inject(MatSnackBar);

  protected readonly history = signal<SyncRunDto[]>([]);
  protected readonly lastResult = signal<SyncResultDto | null>(null);
  protected readonly syncing = signal(false);
  protected readonly merging = signal(false);
  protected readonly selectedFileName = signal<string | null>(null);

  private selectedFile: File | null = null;

  protected readonly columns = ['type', 'startedAt', 'counts', 'summary'];

  ngOnInit(): void {
    this.loadHistory();
  }

  loadHistory(): void {
    this.api.getHistory().subscribe({
      next: (h) => this.history.set(h),
      error: () => this.snack.open('Failed to load history.', 'Dismiss', { duration: 4000 }),
    });
  }

  syncSchema(): void {
    this.syncing.set(true);
    this.api.syncSchema().subscribe({
      next: (r) => {
        this.lastResult.set(r);
        this.syncing.set(false);
        this.snack.open('Schema sync complete.', 'OK', { duration: 2500 });
        this.loadHistory();
      },
      error: () => {
        this.syncing.set(false);
        this.snack.open('Schema sync failed.', 'Dismiss', { duration: 4000 });
      },
    });
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.selectedFile = input.files?.[0] ?? null;
    this.selectedFileName.set(this.selectedFile?.name ?? null);
  }

  mergeMetadata(): void {
    if (!this.selectedFile) {
      this.snack.open('Choose a metadata JSON file first.', 'OK', { duration: 3000 });
      return;
    }
    this.merging.set(true);
    this.api.mergeMetadata(this.selectedFile).subscribe({
      next: (r) => {
        this.lastResult.set(r);
        this.merging.set(false);
        this.snack.open('Metadata merged.', 'OK', { duration: 2500 });
        this.loadHistory();
      },
      error: (err) => {
        this.merging.set(false);
        const msg = typeof err?.error === 'string' ? err.error : 'Metadata merge failed.';
        this.snack.open(msg, 'Dismiss', { duration: 5000 });
      },
    });
  }
}
