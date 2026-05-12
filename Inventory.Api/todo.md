Prueba Técnica - MiniSistema de Gestión de Inventario
Descripción del Proyecto

Desarrollar una aplicación web básica para gestionar el inventario de productos de la empresa CCL. La aplicación permitirá a los usuarios autenticados registrar entradas y salidas de productos, así como consultar el inventario.

Requisitos
Backend: C# (.NET Core 9) y PostgreSQL
1. Autenticación:
Implementar autenticación con JWT (Bearer Token).
Proteger los endpoints para que solo usuarios autenticados puedan acceder.
2. API RESTful:

Endpoints mínimos:

POST /auth/login: Autenticación de usuario (credenciales fijas en memoria).
POST /productos/movimiento: Registrar entrada/salida de productos.
GET /productos/inventario: Consultar el estado actual del inventario.
3. Base de Datos:
1 tabla: productos (id, nombre, cantidad).
Datos iniciales cargados manualmente (sin migraciones complejas).
CRUD manejado directamente con Entity Framework Core.
4. Simplificación:
Sin procedimientos almacenados, solo consultas básicas con EF Core.
Sin Docker, solo configuración local con PostgreSQL.
Frontend: Angular (v19) con TypeScript
1. Interfaz:
Login básico con JWT (sin recuperación de contraseña ni registro de usuario).
Pantalla para registrar movimiento de productos (entrada/salida).
Pantalla para consultar el inventario (lista de productos y cantidades).
Validaciones básicas en formularios (campos obligatorios).
Entrega
Código fuente en GitHub con los commits descriptivos de ejecución de cada etapa del sistema.
Instrucciones en un README.md para correr el proyecto en local.
(Opcional) Video corto mostrando el flujo funcional (si queda tiempo).
Evaluación
Backend: Seguridad básica (JWT), estructur
a limpia y endpoints funcionales.
Frontend: Login e interacción simple con la API.
Código y Git: Claridad y organización en la implementación.
Documentación mínima: Pasos para ejecutar el proyecto.