import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ApiService } from '../../core/api.service';
import { EntityDetailDto, FieldDto, UpdateFieldDto } from '../../core/models';
import { FieldEditDialogComponent } from './field-edit-dialog.component';

@Component({
  selector: 'app-entity-detail',
  imports: [
    CommonModule,
    RouterLink,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSlideToggleModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatTooltipModule,
    MatProgressBarModule,
    MatDialogModule,
  ],
  templateUrl: './entity-detail.component.html',
  styleUrl: './entity-detail.component.scss',
})
export class EntityDetailComponent implements OnInit {
  private readonly api = inject(ApiService);
  private readonly route = inject(ActivatedRoute);
  private readonly fb = inject(FormBuilder);
  private readonly dialog = inject(MatDialog);
  private readonly snack = inject(MatSnackBar);

  protected readonly entity = signal<EntityDetailDto | null>(null);
  protected readonly loading = signal(false);
  protected readonly saving = signal(false);

  protected readonly columns = [
    'businessName',
    'physicalColumnName',
    'type',
    'visible',
    'pii',
    'sensitivity',
    'source',
    'actions',
  ];

  protected readonly form = this.fb.nonNullable.group({
    businessName: ['', Validators.required],
    description: [''],
    isVisible: [true],
  });

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.load(id);
  }

  load(id: number): void {
    this.loading.set(true);
    this.api.getEntity(id).subscribe({
      next: (e) => {
        this.entity.set(e);
        this.form.patchValue({
          businessName: e.businessName,
          description: e.description ?? '',
          isVisible: e.isVisible,
        });
        this.loading.set(false);
      },
      error: () => {
        this.snack.open('Failed to load entity.', 'Dismiss', { duration: 4000 });
        this.loading.set(false);
      },
    });
  }

  saveEntity(): void {
    const e = this.entity();
    if (!e || this.form.invalid) return;
    this.saving.set(true);
    const v = this.form.getRawValue();
    this.api
      .updateEntity(e.id, { businessName: v.businessName, description: v.description || null, isVisible: v.isVisible })
      .subscribe({
        next: (updated) => {
          this.entity.set(updated);
          this.saving.set(false);
          this.snack.open('Entity saved.', 'OK', { duration: 2500 });
        },
        error: () => {
          this.saving.set(false);
          this.snack.open('Save failed.', 'Dismiss', { duration: 4000 });
        },
      });
  }

  editField(field: FieldDto): void {
    const ref = this.dialog.open(FieldEditDialogComponent, { data: field });
    ref.afterClosed().subscribe((dto: UpdateFieldDto | undefined) => {
      if (!dto) return;
      this.api.updateField(field.id, dto).subscribe({
        next: () => {
          const id = this.entity()?.id;
          if (id) this.load(id);
          this.snack.open('Field saved.', 'OK', { duration: 2500 });
        },
        error: () => this.snack.open('Field save failed.', 'Dismiss', { duration: 4000 }),
      });
    });
  }
}
