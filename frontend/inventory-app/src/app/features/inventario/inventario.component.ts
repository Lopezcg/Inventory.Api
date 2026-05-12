import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { InventarioItem, ProductosService } from '../../core/productos.service';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-inventario',
  imports: [CommonModule, RouterLink],
  templateUrl: './inventario.component.html',
  styleUrl: './inventario.component.css'
})
export class InventarioComponent {
  loading = false;
  errorMessage = '';
  inventario: InventarioItem[] = [];

  constructor(private readonly productosService: ProductosService) {
    this.cargarInventario();
  }

  cargarInventario(): void {
    this.loading = true;
    this.errorMessage = '';

    this.productosService.obtenerInventario().subscribe({
      next: (items) => {
        this.inventario = items;
        this.loading = false;
      },
      error: (error) => {
        this.errorMessage = error?.error ?? 'No se pudo cargar el inventario.';
        this.loading = false;
      }
    });
  }
}
