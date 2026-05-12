# MiniSistema de Gestión de Inventario (CCL)

Monorepo con:

- **Backend**: .NET 9 (`Inventory.Api`) + PostgreSQL + JWT Bearer
- **Frontend**: Angular 19 (`frontend/inventory-app`)

## Estructura

```text
CCL-Prueba/
├─ Inventory.Api/              # API REST
├─ frontend/
│  └─ inventory-app/           # Angular app
├─ scripts/                    # scripts utilitarios (opcional)
├─ .env.example
└─ CCL-Prueba.sln
```

## Requisitos

- .NET SDK 9
- Node.js 20+ y npm
- PostgreSQL local

## Base de datos

La API usa una tabla existente: `productos (id, nombre, cantidad)`.

Ejemplo SQL:

```sql
CREATE TABLE IF NOT EXISTS productos (
  id INT PRIMARY KEY,
  nombre VARCHAR(150) NOT NULL,
  cantidad INT NOT NULL
);
```

> Los datos iniciales se cargan manualmente en PostgreSQL.

### Inicialización automática (Windows/PowerShell)

**Script completo** (recomendado): crea BD + tabla + seed en una sola ejecución:

```powershell
.\scripts\setup-db-complete.ps1 -Database inventory_db -Username postgres -Password TU_PASSWORD
```

O si tienes `DB_PASSWORD` en el entorno:

```powershell
.\scripts\setup-db-complete.ps1 -Database inventory_db -Username postgres
```

Datos semilla que se insertan automáticamente:
- Teclado (10 unidades)
- Mouse (15 unidades)
- Monitor (5 unidades)
- Laptop (3 unidades)
- Cable HDMI (50 unidades)

**Script alternativo** (solo tabla + seed en BD existente):

```powershell
.\scripts\init-db.ps1 -Database TU_BD -Username postgres -Password TU_PASSWORD -SeedData
```

## Configuración de variables (`.env`)

En la **raíz del repo** (`CCL-Prueba/.env`), crea:

```env
# Opción recomendada: cadena completa
DB_CONNECTION_STRING=Host=localhost;Port=5432;Database=TU_BD;Username=postgres;Password=TU_PASSWORD

# Opción alternativa: solo password (usa host/db/user de appsettings)
# DB_PASSWORD=TU_PASSWORD
```

También puedes usar `./.env.example` como guía.

## Ejecutar backend

Terminal 1:

```bash
cd Inventory.Api
dotnet run
```

API en desarrollo: `http://localhost:5038`

## Ejecutar frontend

Terminal 2:

```bash
cd frontend/inventory-app
npm install
npm start
```

Frontend: `http://localhost:4200`

## CORS

El backend está configurado para permitir:

- `http://localhost:4200`
- `http://127.0.0.1:4200`

## Autenticación y flujo básico

Credenciales de login (por defecto, configurables en `Inventory.Api/appsettings.Development.json`):

- Usuario: `admin`
- Contraseña: `ChangeThisPassword!`

Flujo:

1. Iniciar sesión en `/login`.
2. Registrar movimiento en `/movimiento` (`entrada` o `salida`).
3. Consultar inventario en `/inventario`.

## Endpoints backend

- `POST /auth/login`
- `POST /productos/movimiento` (requiere Bearer token)
- `GET /productos/inventario` (requiere Bearer token)
