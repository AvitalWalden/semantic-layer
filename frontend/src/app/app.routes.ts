import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'management' },
  {
    path: 'management',
    loadComponent: () =>
      import('./features/management/entity-list.component').then((m) => m.EntityListComponent),
  },
  {
    path: 'management/:id',
    loadComponent: () =>
      import('./features/management/entity-detail.component').then((m) => m.EntityDetailComponent),
  },
  {
    path: 'sync',
    loadComponent: () => import('./features/sync/sync.component').then((m) => m.SyncComponent),
  },
  {
    path: 'explorer',
    loadComponent: () =>
      import('./features/explorer/explorer.component').then((m) => m.ExplorerComponent),
  },
  { path: '**', redirectTo: 'management' },
];
