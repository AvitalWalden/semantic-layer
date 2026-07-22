import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ApiService } from '../../core/api.service';
import { DataResultDto, EntityDto } from '../../core/models';

@Component({
  selector: 'app-explorer',
  imports: [
    CommonModule,
    FormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatSelectModule,
    MatTableModule,
    MatPaginatorModule,
    MatIconModule,
    MatProgressBarModule,
    MatTooltipModule,
  ],
  templateUrl: './explorer.component.html',
  styleUrl: './explorer.component.scss',
})
export class ExplorerComponent implements OnInit {
  private readonly api = inject(ApiService);

  protected readonly entities = signal<EntityDto[]>([]);
  protected readonly selectedId = signal<number | null>(null);
  protected readonly data = signal<DataResultDto | null>(null);
  protected readonly loading = signal(false);

  protected pageSize = 10;
  protected pageIndex = 0;

  protected readonly displayedColumns = computed(() => this.data()?.columns.map((c) => c.businessName) ?? []);

  ngOnInit(): void {
    this.api.getEntities(true).subscribe((e) => {
      this.entities.set(e);
      if (e.length > 0) {
        this.selectedId.set(e[0].id);
        this.load();
      }
    });
  }

  onEntityChange(): void {
    this.pageIndex = 0;
    this.load();
  }

  onPage(event: PageEvent): void {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.load();
  }

  load(): void {
    const id = this.selectedId();
    if (id == null) return;
    this.loading.set(true);
    this.api.getData(id, this.pageIndex + 1, this.pageSize).subscribe({
      next: (d) => {
        this.data.set(d);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  isDerived(col: string): boolean {
    return this.data()?.columns.find((c) => c.businessName === col)?.isDerived ?? false;
  }

  unit(col: string): string | null | undefined {
    return this.data()?.columns.find((c) => c.businessName === col)?.unit;
  }

  format(value: unknown): string {
    if (value === null || value === undefined) return '—';
    if (typeof value === 'boolean') return value ? 'Yes' : 'No';
    if (typeof value === 'string' && /^\d{4}-\d{2}-\d{2}T/.test(value)) {
      return value.substring(0, 10);
    }
    return String(value);
  }
}
