param(
    [string]$HostName = "localhost",
    [int]$Port = 5432,
    [string]$Database = "inventory_db_auto",
    [string]$Username = "postgres",
    [string]$Password = ""
)

$ErrorActionPreference = "Stop"

# Validar psql
if (-not (Get-Command psql -ErrorAction SilentlyContinue)) {
    throw "No se encontro 'psql' en el PATH. Instala PostgreSQL client tools o agrega psql al PATH."
}

# Obtener password
if ([string]::IsNullOrWhiteSpace($Password)) {
    $Password = $env:DB_PASSWORD
}

if ([string]::IsNullOrWhiteSpace($Password)) {
    throw "Debes indicar -Password o definir DB_PASSWORD en el entorno."
}

$env:PGPASSWORD = $Password

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Setup Completo: BD + Tabla + Seed" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

try {
    # 1. Crear la base de datos
    Write-Host "1. Creando base de datos: $Database..." -ForegroundColor Yellow
    $output = & psql -h $HostName -p $Port -U $Username -d postgres -tc "SELECT 1 FROM pg_database WHERE datname = '$Database'" 2>&1
    
    if ($output -like "*1*") {
        Write-Host "   OK Base de datos ya existe" -ForegroundColor Green
    } else {
        & psql -h $HostName -p $Port -U $Username -d postgres -c "CREATE DATABASE $Database;" 2>&1 | Out-Null
        Write-Host "   OK Base de datos creada" -ForegroundColor Green
    }

    # 2. Crear la tabla
    Write-Host "2. Creando tabla 'productos'..." -ForegroundColor Yellow
    $createTableSql = 'CREATE TABLE IF NOT EXISTS productos (id INT PRIMARY KEY, nombre VARCHAR(150) NOT NULL, cantidad INT NOT NULL);'
    & psql -h $HostName -p $Port -U $Username -d $Database -c $createTableSql 2>&1 | Out-Null
    Write-Host "   OK Tabla creada/verificada" -ForegroundColor Green

    # 3. Insertar datos semilla
    Write-Host "3. Insertando datos semilla..." -ForegroundColor Yellow
    $seedSql = "INSERT INTO productos (id, nombre, cantidad) VALUES (1, 'Teclado', 10), (2, 'Mouse', 15), (3, 'Monitor', 5), (4, 'Laptop', 3), (5, 'Cable HDMI', 50) ON CONFLICT (id) DO NOTHING;"
    & psql -h $HostName -p $Port -U $Username -d $Database -c $seedSql 2>&1 | Out-Null
    Write-Host "   OK Datos semilla insertados" -ForegroundColor Green

    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "OK Setup completado correctamente!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "BD: $Database" -ForegroundColor Cyan
    Write-Host "Host: $($HostName):$Port" -ForegroundColor Cyan
    Write-Host "Usuario: $Username" -ForegroundColor Cyan

}
catch {
    Write-Host ""
    Write-Host "ERROR Error durante la inicializacion:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    exit 1
}
finally {
    Remove-Item Env:PGPASSWORD -ErrorAction SilentlyContinue
}
