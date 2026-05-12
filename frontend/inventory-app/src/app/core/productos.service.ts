import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';

export interface MovimientoRequest {
  productoId: number;
  tipo: 'entrada' | 'salida';
  cantidad: number;
}

export interface MovimientoResponse {
  productoId: number;
  nombre: string;
  tipo: string;
  cantidadMovida: number;
  stockActual: number;
}

export interface InventarioItem {
  id: number;
  nombre: string;
  cantidad: number;
}

@Injectable({ providedIn: 'root' })
export class ProductosService {
  private readonly apiUrl = 'http://localhost:5038';

  constructor(
    private readonly http: HttpClient,
    private readonly authService: AuthService
  ) {}

  registrarMovimiento(payload: MovimientoRequest): Observable<MovimientoResponse> {
    return this.http.post<MovimientoResponse>(`${this.apiUrl}/productos/movimiento`, payload, {
      headers: this.getAuthHeaders()
    });
  }

  obtenerInventario(): Observable<InventarioItem[]> {
    return this.http.get<InventarioItem[]>(`${this.apiUrl}/productos/inventario`, {
      headers: this.getAuthHeaders()
    });
  }

  private getAuthHeaders(): HttpHeaders | undefined {
    const token = this.authService.getToken();
    return token ? new HttpHeaders({ Authorization: `Bearer ${token}` }) : undefined;
  }
}
