import { Routes } from '@angular/router';
import { LoginComponent } from './features/login/login.component';
import { MovimientoComponent } from './features/movimiento/movimiento.component';
import { InventarioComponent } from './features/inventario/inventario.component';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'login' },
  { path: 'login', component: LoginComponent },
  { path: 'movimiento', component: MovimientoComponent },
  { path: 'inventario', component: InventarioComponent }
];
