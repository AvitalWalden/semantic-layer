import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { FieldDto, SensitivityLevel, UpdateFieldDto } from '../../core/models';

@Component({
  selector: 'app-field-edit-dialog',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatSlideToggleModule,
    MatButtonModule,
    MatIconModule,
  ],
  template: `
    <h2 mat-dialog-title>Edit field: {{ data.physicalColumnName }}</h2>
    <mat-dialog-content>
      <form [formGroup]="form" class="field-form">
        <mat-form-field appearance="outline">
          <mat-label>Business Name</mat-label>
          <input matInput formControlName="businessName" />
        </mat-form-field>

        <mat-form-field appearance="outline">
          <mat-label>Description</mat-label>
          <textarea matInput rows="2" formControlName="description"></textarea>
        </mat-form-field>

        <div class="row">
          <mat-form-field appearance="outline">
            <mat-label>Sensitivity</mat-label>
            <mat-select formControlName="sensitivityLevel">
              @for (level of levels; track level) {
                <mat-option [value]="level">{{ level }}</mat-option>
              }
            </mat-select>
          </mat-form-field>

          <mat-form-field appearance="outline">
            <mat-label>Unit</mat-label>
            <input matInput formControlName="unit" placeholder="e.g. USD" />
          </mat-form-field>
        </div>

        <div class="toggles">
          <mat-slide-toggle formControlName="isVisible">Visible to business users</mat-slide-toggle>
          <mat-slide-toggle formControlName="isPii">Personally identifiable (PII)</mat-slide-toggle>
        </div>

        @if (data.isDerived) {
          <div class="derived-note">
            <mat-icon>functions</mat-icon>
            <span>Derived field &middot; <code>{{ data.derivedExpression }}</code></span>
          </div>
        }
      </form>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button mat-dialog-close>Cancel</button>
      <button mat-flat-button color="primary" [disabled]="form.invalid" (click)="save()">Save</button>
    </mat-dialog-actions>
  `,
  styles: [
    `
      .field-form {
        display: flex;
        flex-direction: column;
        min-width: 420px;
        /* Space so the first field's floating label is not clipped by the dialog content edge. */
        padding-top: 10px;
      }
      .row {
        display: flex;
        gap: 12px;
      }
      .row mat-form-field {
        flex: 1;
      }
      .toggles {
        display: flex;
        flex-direction: column;
        gap: 12px;
        margin: 4px 0 12px;
      }
      .derived-note {
        display: flex;
        align-items: center;
        gap: 8px;
        color: #5b6b7b;
      }
      code {
        background: #eef1f5;
        padding: 2px 6px;
        border-radius: 4px;
      }
    `,
  ],
})
export class FieldEditDialogComponent {
  private readonly fb = inject(FormBuilder);
  private readonly ref = inject(MatDialogRef<FieldEditDialogComponent>);
  protected readonly data = inject<FieldDto>(MAT_DIALOG_DATA);

  protected readonly levels: SensitivityLevel[] = ['Public', 'Internal', 'Confidential', 'Restricted'];

  protected readonly form = this.fb.nonNullable.group({
    businessName: [this.data.businessName, Validators.required],
    description: [this.data.description ?? ''],
    isVisible: [this.data.isVisible],
    isPii: [this.data.isPii],
    sensitivityLevel: [this.data.sensitivityLevel],
    unit: [this.data.unit ?? ''],
  });

  save(): void {
    const v = this.form.getRawValue();
    const dto: UpdateFieldDto = {
      businessName: v.businessName,
      description: v.description || null,
      isVisible: v.isVisible,
      isPii: v.isPii,
      sensitivityLevel: v.sensitivityLevel,
      unit: v.unit || null,
      displayFormat: this.data.displayFormat ?? null,
    };
    this.ref.close(dto);
  }
}
