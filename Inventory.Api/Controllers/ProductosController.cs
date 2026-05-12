using Inventory.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Controllers;

[ApiController]
[Route("productos")]
[Authorize]
public class ProductosController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public ProductosController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("inventario")]
    public async Task<ActionResult<IReadOnlyList<InventarioItemResponse>>> ObtenerInventario()
    {
        var inventario = await _dbContext.Productos
            .AsNoTracking()
            .OrderBy(p => p.Nombre)
            .Select(p => new InventarioItemResponse(p.Id, p.Nombre, p.Cantidad))
            .ToListAsync();

        return Ok(inventario);
    }

    [HttpPost("movimiento")]
    public async Task<ActionResult<MovimientoResponse>> RegistrarMovimiento([FromBody] MovimientoRequest request)
    {
        if (request.Cantidad <= 0)
        {
            return BadRequest("La cantidad debe ser mayor que cero.");
        }

        var tipo = request.Tipo.Trim().ToLowerInvariant();
        if (tipo != "entrada" && tipo != "salida")
        {
            return BadRequest("El tipo de movimiento debe ser 'entrada' o 'salida'.");
        }

        var producto = await _dbContext.Productos.FirstOrDefaultAsync(p => p.Id == request.ProductoId);
        if (producto is null)
        {
            return NotFound($"No existe un producto con id {request.ProductoId}.");
        }

        if (tipo == "salida" && producto.Cantidad < request.Cantidad)
        {
            return BadRequest("No hay stock suficiente para registrar la salida.");
        }

        if (tipo == "entrada")
        {
            producto.Cantidad += request.Cantidad;
        }
        else
        {
            producto.Cantidad -= request.Cantidad;
        }

        await _dbContext.SaveChangesAsync();

        return Ok(new MovimientoResponse(producto.Id, producto.Nombre, tipo, request.Cantidad, producto.Cantidad));
    }

    public sealed record MovimientoRequest(int ProductoId, string Tipo, int Cantidad);

    public sealed record MovimientoResponse(int ProductoId, string Nombre, string Tipo, int CantidadMovida, int StockActual);

    public sealed record InventarioItemResponse(int Id, string Nombre, int Cantidad);
}
