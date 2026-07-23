export type ObjectStatus = 'Active' | 'Orphaned';
export type SensitivityLevel = 'Public' | 'Internal' | 'Confidential' | 'Restricted';
export type SyncType = 'Schema' | 'Metadata';

export interface EntityDto {
  id: number;
  physicalTableName: string;
  businessName: string;
  description?: string | null;
  isVisible: boolean;
  status: ObjectStatus;
  isUserModified: boolean;
  fieldCount: number;
}

export interface FieldDto {
  id: number;
  physicalColumnName: string;
  businessName: string;
  description?: string | null;
  physicalDataType?: string | null;
  isVisible: boolean;
  isPii: boolean;
  sensitivityLevel: SensitivityLevel;
  unit?: string | null;
  displayFormat?: string | null;
  isDerived: boolean;
  derivedExpression?: string | null;
  status: ObjectStatus;
  sortOrder: number;
  isUserModified: boolean;
}

export interface EntityDetailDto {
  id: number;
  physicalTableName: string;
  businessName: string;
  description?: string | null;
  isVisible: boolean;
  status: ObjectStatus;
  primaryKeyColumn?: string | null;
  isUserModified: boolean;
  fields: FieldDto[];
}

export interface UpdateEntityDto {
  businessName: string;
  description?: string | null;
  isVisible: boolean;
}

export interface UpdateFieldDto {
  businessName: string;
  description?: string | null;
  isVisible: boolean;
  isPii: boolean;
  sensitivityLevel: SensitivityLevel;
  unit?: string | null;
  displayFormat?: string | null;
}

export interface SyncResultDto {
  type: SyncType;
  entitiesAdded: number;
  entitiesRemoved: number;
  fieldsAdded: number;
  fieldsUpdated: number;
  fieldsRemoved: number;
  summary: string;
}

export interface DataColumnDto {
  businessName: string;
  unit?: string | null;
  isDerived: boolean;
}

export interface DataResultDto {
  entityId: number;
  entityBusinessName: string;
  columns: DataColumnDto[];
  rows: Array<Record<string, unknown>>;
  page: number;
  pageSize: number;
  totalRows: number;
}
