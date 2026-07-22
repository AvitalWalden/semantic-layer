import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import {
  DataResultDto,
  EntityDetailDto,
  EntityDto,
  FieldDto,
  SyncResultDto,
  SyncRunDto,
  UpdateEntityDto,
  UpdateFieldDto,
} from './models';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly http = inject(HttpClient);
  private readonly base = '/api';

  getEntities(onlyVisible = false): Observable<EntityDto[]> {
    const params = new HttpParams().set('onlyVisible', onlyVisible);
    return this.http.get<EntityDto[]>(`${this.base}/semantic/entities`, { params });
  }

  getEntity(id: number): Observable<EntityDetailDto> {
    return this.http.get<EntityDetailDto>(`${this.base}/semantic/entities/${id}`);
  }

  updateEntity(id: number, dto: UpdateEntityDto): Observable<EntityDetailDto> {
    return this.http.put<EntityDetailDto>(`${this.base}/semantic/entities/${id}`, dto);
  }

  updateField(id: number, dto: UpdateFieldDto): Observable<FieldDto> {
    return this.http.put<FieldDto>(`${this.base}/semantic/fields/${id}`, dto);
  }

  syncSchema(): Observable<SyncResultDto> {
    return this.http.post<SyncResultDto>(`${this.base}/sync/schema`, {});
  }

  mergeMetadata(file: File): Observable<SyncResultDto> {
    const form = new FormData();
    form.append('file', file);
    return this.http.post<SyncResultDto>(`${this.base}/sync/metadata`, form);
  }

  getHistory(take = 50): Observable<SyncRunDto[]> {
    const params = new HttpParams().set('take', take);
    return this.http.get<SyncRunDto[]>(`${this.base}/sync/history`, { params });
  }

  getData(entityId: number, page: number, pageSize: number): Observable<DataResultDto> {
    const params = new HttpParams().set('page', page).set('pageSize', pageSize);
    return this.http.get<DataResultDto>(`${this.base}/data/${entityId}`, { params });
  }
}
