import { Routes } from '@angular/router';
import { CustomerDashboard } from './customer-dashboard/customer-dashboard';

export const routes: Routes = [
  { path: '', component: CustomerDashboard },
  { path: '**', redirectTo: '' },
];
