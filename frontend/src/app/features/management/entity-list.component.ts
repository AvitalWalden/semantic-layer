import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ApiService } from '../../core/api.service';
import { EntityDto } from '../../core/models';

@Component({
  selector: 'app-entity-list',
  imports: [
    CommonModule,
    RouterLink,
    MatCardModule,
    MatTableModule,
    MatIconModule,
    MatButtonModule,
    MatChipsModule,
    MatProgressBarModule,
    MatTooltipModule,
  ],
  templateUrl: './entity-list.component.html',
  styleUrl: './entity-list.component.scss',
})
export class EntityListComponent implements OnInit {
  private readonly api = inject(ApiService);

  protected readonly entities = signal<EntityDto[]>([]);
  protected readonly loading = signal(false);
  protected readonly error = signal<string | null>(null);

  protected readonly columns = [
    'businessName',
    'physicalTableName',
    'fieldCount',
    'visible',
    'status',
    'source',
    'actions',
  ];

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.error.set(null);
    this.api.getEntities(false).subscribe({
      next: (data) => {
        this.entities.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load entities. Is the backend running?');
        this.loading.set(false);
      },
    });
  }
}
