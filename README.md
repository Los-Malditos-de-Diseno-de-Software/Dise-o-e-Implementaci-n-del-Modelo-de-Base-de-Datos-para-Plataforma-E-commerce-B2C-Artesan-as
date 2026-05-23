# 🏔️🎨 E-Commerce Artesanías Cusco

¡Bienvenido al repositorio oficial del **E-Commerce de Artesanías de Cusco**! Una plataforma transaccional de comercio electrónico B2C premium diseñada y desarrollada bajo los estándares más exigentes de la Ingeniería de Software. La aplicación conecta directamente a artesanos cusqueños con compradores globales, permitiendo la exhibición y adquisición de piezas únicas con flujos altamente seguros.

Este proyecto está bajo el desarrollo de la organización **Los Malditos de Diseño de Software**.

---

## 🧭 1. Arquitectura del Sistema

La solución está construida sobre una arquitectura desacoplada y robusta que garantiza mantenibilidad, escalabilidad y tolerancia a fallos:

### 🖥️ Backend: Clean Architecture (.NET 8)
El backend está estructurado siguiendo estrictamente los principios de **Clean Architecture** y el patrón de diseño **Domain-Driven Design (DDD)** simplificado:
*   **Artesanias.Domain:** Contiene las entidades puras de negocio, interfaces de repositorio y lógica central libre de dependencias externas.
*   **Artesanias.Application:** Define los casos de uso del sistema, DTOs, validaciones con FluentValidation y manejadores de comandos/consultas (CQRS).
*   **Artesanias.Infrastructure:** Implementa el acceso a datos (Entity Framework Core, Unit of Work, Repositorios) y la integración con servicios externos como Stripe para pagos.
*   **Artesanias.Api:** Punto de entrada de la aplicación HTTP, middleware global de excepciones (RFC 7807) y configuración de JWT.

### 🎨 Frontend: React Single Page Application (SPA)
El frontend es modular, tipado estrictamente con TypeScript y diseñado con un enfoque visual premium:
*   **Gestión del Estado del Servidor:** Implementado con **TanStack Query** (React Query) para sincronización en tiempo real.
*   **Gestión del Estado Global:** **Zustand** para persistencia ligera de sesiones anónimas.
*   **Diseño Visual:** **Tailwind CSS** con soporte nativo para **Modo Oscuro (Dark Mode)** por defecto.
*   **Optimización de Medios:** Renderizado eficiente de imágenes directamente desde Base64 almacenadas en base de datos.

---

## 🧠 2. Decisiones de Diseño Clave

Para cumplir con las especificaciones del SSoT (Single Source of Truth) en base de datos, implementamos tres soluciones arquitectónicas cruciales:

1.  **Carrito de Compras Persistido en SQL (`ShoppingCarts`)**: 
    El frontend genera un `SessionId` (UUID) almacenado localmente. Este identificador se transmite en los headers (`X-Session-Id`) para persistir y sincronizar de forma temporal el carrito de compras en la base de datos SQL Server, incluso para visitantes anónimos. Una vez autenticado el usuario, el carrito se asocia a su cuenta.
2.  **Almacenamiento de Imágenes Centralizado (`VARBINARY(MAX)`)**:
    Evitando la dependencia de servicios de storage externos para mantener el SSoT, las imágenes se almacenan como datos binarios en la tabla `ProductImages`. El backend transforma estos binarios en formato **Base64** en los DTOs de respuesta para su renderizado óptimo en el cliente mediante etiquetas `<img src="data:image/jpeg;base64,..." />`.
3.  **Transacciones ACID en Pagos e Inventario**:
    Todo el flujo de checkout, verificación y registro de Stripe se ejecuta bajo transacciones con propiedades ACID. La confirmación del pago en `PaymentTransactions`, el decremento del stock en `Productos`, y el vaciado del carrito se procesan en un único bloque transaccional SQL indivisible.

---

## 📊 3. Modelo de Base de Datos (Esquema SSoT)

El sistema utiliza **SQL Server** con la clave primaria genérica `Id` en formato `UNIQUEIDENTIFIER` (UUID) para todas las tablas.

| Entidad | Descripción | Campos Clave |
| :--- | :--- | :--- |
| **Artesanos** | Registro de maestros artesanos de Cusco | `Id`, `Nombre`, `HistoriaBiografia`, `ComunidadOrigen`, `IsDeleted` |
| **Productos** | Catálogo de obras de arte | `Id`, `ArtesanoId` (FK), `Nombre`, `Precio`, `Stock`, `EsUnico`, `IsDeleted` |
| **ProductImages** | Almacenamiento binario de fotos | `Id`, `ProductoId` (FK), `ImageData` (VARBINARY), `ContentType` |
| **ShoppingCarts** | Carritos de compra persistidos | `Id`, `SessionId` (UUID), `UsuarioId` (Null FK), `UltimaActualizacion` |
| **CartItems** | Detalles de productos en carritos | `Id`, `ShoppingCartId` (FK), `ProductoId` (FK), `Cantidad`, `PrecioUnitarioCongelado` |
| **Orders** | Historial de órdenes de compra | `Id`, `UsuarioId` (FK), `Total`, `EstadoPedido` |
| **PaymentTransactions** | Auditoría y estado de pasarela de pagos | `Id`, `OrderId` (FK), `MetodoPago`, `EstadoPago`, `ReferenciaPasarela`, `PayloadPasarela` |

---

## 📂 4. Estructura de Archivos del Proyecto

```text
📦 Artesanias-ecomerce
 ┣ 📂 backend
 ┃ ┣ 📂 src
 ┃ ┃ ┣ 📂 Artesanias.Domain          # Entidades y contratos base
 ┃ ┃ ┣ 📂 Artesanias.Application     # Casos de uso, validaciones y CQRS
 ┃ ┃ ┣ 📂 Artesanias.Infrastructure  # DBContext, Repositorios, Stripe, JWT
 ┃ ┃ ┗ 📂 Artesanias.Api             # Controladores y Middlewares
 ┃ ┣ 📂 tests
 ┃ ┃ ┣ 📂 Artesanias.UnitTests       # Pruebas unitarias (xUnit, Moq)
 ┃ ┃ ┗ 📂 Artesanias.IntegrationTests# Pruebas de integración
 ┃ ┗ 📜 Artesanias.sln
 ┣ 📂 frontend
 ┃ ┣ 📂 src
 ┃ ┃ ┣ 📂 api                        # Consumo de APIs con Axios
 ┃ ┃ ┣ 📂 components                 # UI atómica y Features complejas
 ┃ ┃ ┣ 📂 hooks                      # Custom React Hooks
 ┃ ┃ ┣ 📂 layouts                    # Plantillas de diseño (Público y Admin)
 ┃ ┃ ┣ 📂 pages                      # Páginas del SPA
 ┃ ┃ ┣ 📂 store                      # Estados globales (Zustand)
 ┃ ┃ ┗ 📂 types                      # Mapeo de tipos TypeScript 1:1 con Backend
 ┃ ┣ 📜 package.json
 ┃ ┗ 📜 vite.config.ts
 ┣ 📜 .gitignore                     # Configuración de archivos excluidos
 ┗ 📜 README.md                      # Documentación del proyecto
```

---

## 🚀 5. Primeros Pasos

### Requisitos Previos
*   .NET SDK 8.0+
*   Node.js v18+ y npm / pnpm
*   SQL Server Express o LocalDB

### Configuración del Backend
1. Navega al directorio del backend:
   ```bash
   cd backend/src/Artesanias.Api
   ```
2. Modifica el archivo `appsettings.json` para agregar tu cadena de conexión a SQL Server y las claves de desarrollo de Stripe.
3. Ejecuta las migraciones de EF Core para crear la base de datos:
   ```bash
   dotnet ef database update
   ```
4. Inicia el servidor:
   ```bash
   dotnet run
   ```

### Configuración del Frontend
1. Navega al directorio del frontend:
   ```bash
   cd frontend
   ```
2. Instala las dependencias:
   ```bash
   npm install
   ```
3. Crea un archivo `.env` basado en `.env.example` y configura la URL de la API:
   ```env
   VITE_API_URL=https://localhost:7124
   ```
4. Inicia el servidor de desarrollo:
   ```bash
   npm run dev
   ```

---

## 🤝 6. Flujo de Trabajo y Contribución

El equipo de desarrollo utiliza **GitHub Flow** y respeta estrictamente el estándar de **Conventional Commits**:

*   **Ramas del repositorio:**
    *   `main`: Producción, estable y verificado.
    *   `develop`: Integración de características.
    *   `feature/nombre-corto`: Desarrollo de nuevas funcionalidades.
*   **Convenciones de Commits:**
    *   `feat: ...` para nuevas características.
    *   `fix: ...` para corrección de bugs.
    *   `docs: ...` para cambios en la documentación.
    *   `refactor: ...` para optimizaciones de código.
