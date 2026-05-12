import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MovimientoResponse, ProductosService } from '../../core/productos.service';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-movimiento',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './movimiento.component.html',
  styleUrl: './movimiento.component.css'
})
export class MovimientoComponent {
  private readonly fb = inject(FormBuilder);

  loading = false;
  errorMessage = '';
  result: MovimientoResponse | null = null;

  readonly form = this.fb.group({
    productoId: [null as number | null, [Validators.required, Validators.min(1)]],
    tipo: ['entrada' as 'entrada' | 'salida', Validators.required],
    cantidad: [null as number | null, [Validators.required, Validators.min(1)]]
  });

  constructor(private readonly productosService: ProductosService) {}

  submit(): void {
    if (this.form.invalid || this.loading) {
      this.form.markAllAsTouched();
      return;
    }

    const productoId = this.form.controls.productoId.value;
    const tipo = this.form.controls.tipo.value;
    const cantidad = this.form.controls.cantidad.value;
    if (productoId == null || tipo == null || cantidad == null) {
      return;
    }

    this.loading = true;
    this.errorMessage = '';
    this.result = null;

    this.productosService
      .registrarMovimiento({ productoId, tipo, cantidad })
      .subscribe({
        next: (response) => {
          this.result = response;
          this.loading = false;
        },
        error: (error) => {
          this.errorMessage = error?.error ?? 'No se pudo registrar el movimiento.';
          this.loading = false;
        }
      });
  }
}
