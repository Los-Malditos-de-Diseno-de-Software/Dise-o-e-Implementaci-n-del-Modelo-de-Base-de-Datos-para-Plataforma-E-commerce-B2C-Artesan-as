# E-Commerce Artesanías Cusco - Architecture Blueprint

## 🧭 1. TIPOLOGÍA Y ARQUITECTURA SELECCIONADA

*   **Tipología:** Web App Compleja (E-Commerce Transaccional) con flujos de concurrencia, persistencia profunda en base de datos y pagos integrados.
*   **Arquitectura Backend:** El sistema respetará estrictamente la Clean Architecture para garantizar un diseño robusto, digno del más alto nivel de la Ingeniería de Sistemas e Informática[cite: 2]. 
*   **Arquitectura Frontend:** Single Page Application (SPA) modularizada y tipada estrictamente.
*   **[ESTRATEGIA_DE_SUPOSICIÓN]:** 
    1. **Carrito en SQL:** Dado que el requerimiento exige el carrito de compras en SQL y existen visitantes no logueados, el frontend generará un `SessionId` (UUID) que enviará en los headers. El backend usará este ID para persistir el carrito en la base de datos de forma temporal hasta que el usuario se autentique.
    2. **Imágenes en SQL:** Guardar imágenes en base de datos puede afectar el rendimiento. Se asume el uso de `VARBINARY(MAX)` en una tabla separada (`ProductImages`) vinculada a la tabla `Productos`, sirviendo las imágenes al frontend convertidas en formato Base64.
    3. **Pagos en SQL:** Todo el estado de la transacción y pasarela se registrará en la tabla `PaymentTransactions` antes, durante y después del webhook.

---

## 🧠 2. INTEGRATION_MANIFEST

~~~json
{
  "IntegrationManifest": {
    "ProjectName": "E-Commerce Artesanias Cusco",
    "Description": "SSoT para entidades de catálogo, carrito, pagos ACID e imágenes centralizadas en BD.",
    "GlobalStandards": {
      "Database": {
        "PrimaryKeyFormat": "Id",
        "Strings": "Longitud explícita, prohibido NVARCHAR(MAX)",
        "Audit": "Obligatorio CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted"
      }
    },
    "Entities": {
      "Artesanos": {
        "Id": { "sql": "UNIQUEIDENTIFIER", "csharp": "Guid", "ts": "string" },
        "Nombre": { "sql": "NVARCHAR(150)", "csharp": "string", "ts": "string" },
        "HistoriaBiografia": { "sql": "NVARCHAR(2000)", "csharp": "string", "ts": "string" },
        "ComunidadOrigen": { "sql": "NVARCHAR(100)", "csharp": "string", "ts": "string" },
        "IsDeleted": { "sql": "BIT", "csharp": "bool", "ts": "boolean" },
        "CreatedAt": { "sql": "DATETIME2", "csharp": "DateTime", "ts": "string" }
      },
      "Productos": {
        "Id": { "sql": "UNIQUEIDENTIFIER", "csharp": "Guid", "ts": "string" },
        "ArtesanoId": { "sql": "UNIQUEIDENTIFIER", "csharp": "Guid", "ts": "string" },
        "Nombre": { "sql": "NVARCHAR(150)", "csharp": "string", "ts": "string" },
        "Descripcion": { "sql": "NVARCHAR(1000)", "csharp": "string", "ts": "string" },
        "Precio": { "sql": "DECIMAL(10,2)", "csharp": "decimal", "ts": "number" },
        "Stock": { "sql": "INT", "csharp": "int", "ts": "number" },
        "EsUnico": { "sql": "BIT", "csharp": "bool", "ts": "boolean" },
        "IsDeleted": { "sql": "BIT", "csharp": "bool", "ts": "boolean" }
      },
      "ProductImages": {
        "Id": { "sql": "UNIQUEIDENTIFIER", "csharp": "Guid", "ts": "string" },
        "ProductoId": { "sql": "UNIQUEIDENTIFIER", "csharp": "Guid", "ts": "string" },
        "ImageData": { "sql": "VARBINARY(MAX)", "csharp": "byte[]", "ts": "string" },
        "ContentType": { "sql": "NVARCHAR(50)", "csharp": "string", "ts": "string" }
      },
      "ShoppingCarts": {
        "Id": { "sql": "UNIQUEIDENTIFIER", "csharp": "Guid", "ts": "string" },
        "SessionId": { "sql": "UNIQUEIDENTIFIER", "csharp": "Guid", "ts": "string" },
        "UsuarioId": { "sql": "UNIQUEIDENTIFIER NULL", "csharp": "Guid?", "ts": "string | null" },
        "UltimaActualizacion": { "sql": "DATETIME2", "csharp": "DateTime", "ts": "string" }
      },
      "CartItems": {
        "Id": { "sql": "UNIQUEIDENTIFIER", "csharp": "Guid", "ts": "string" },
        "ShoppingCartId": { "sql": "UNIQUEIDENTIFIER", "csharp": "Guid", "ts": "string" },
        "ProductoId": { "sql": "UNIQUEIDENTIFIER", "csharp": "Guid", "ts": "string" },
        "Cantidad": { "sql": "INT", "csharp": "int", "ts": "number" },
        "PrecioUnitarioCongelado": { "sql": "DECIMAL(10,2)", "csharp": "decimal", "ts": "number" }
      },
      "Orders": {
        "Id": { "sql": "UNIQUEIDENTIFIER", "csharp": "Guid", "ts": "string" },
        "UsuarioId": { "sql": "UNIQUEIDENTIFIER", "csharp": "Guid", "ts": "string" },
        "Total": { "sql": "DECIMAL(10,2)", "csharp": "decimal", "ts": "number" },
        "EstadoPedido": { "sql": "NVARCHAR(50)", "csharp": "string", "ts": "string" }
      },
      "PaymentTransactions": {
        "Id": { "sql": "UNIQUEIDENTIFIER", "csharp": "Guid", "ts": "string" },
        "OrderId": { "sql": "UNIQUEIDENTIFIER", "csharp": "Guid", "ts": "string" },
        "MetodoPago": { "sql": "NVARCHAR(50)", "csharp": "string", "ts": "string" },
        "EstadoPago": { "sql": "NVARCHAR(50)", "csharp": "string", "ts": "string" },
        "ReferenciaPasarela": { "sql": "NVARCHAR(200)", "csharp": "string", "ts": "string" },
        "PayloadPasarela": { "sql": "NVARCHAR(2000)", "csharp": "string", "ts": "string" }
      }
    },
    "ApiRoutes": {
      "Products": "/api/v1/productos",
      "Cart": "/api/v1/carrito",
      "Orders": "/api/v1/pedidos",
      "PaymentsWebhook": "/api/v1/pagos/webhook"
    },
    "UIUXCore": {
      "Theme": {
        "Support": "Modo Oscuro nativo requerido por defecto",
        "Primary": "#D97736",
        "Secondary": "#2C3E50",
        "BackgroundDark": "#121212"
      }
    }
  }
}
~~~

---

## 🗺️ 3. ROADMAP DE EJECUCIÓN

1.  **Fase 1: Infraestructura y Dominio Centralizado** (Modelado SQL Server de todas las tablas incluyendo Imágenes, EF Core, Repositorios).
2.  **Fase 2: Catálogo e Imágenes** (APIs de productos, guardado y recuperación de `VARBINARY(MAX)` a Base64).
3.  **Fase 3: Flujos de Sesión y Carrito 100% SQL** (Control de concurrencia y persistencia mediante `SessionId`).
4.  **Fase 4: Transaccionalidad de Pagos SQL** (Patrones ACID, Integración de registros detallados de pasarelas en la BD, Webhooks).
5.  **Fase 5: Consumo Web** (React SPA, TanStack Query, interfaces estrictas).
6.  **Fase 6: UI/UX Fino** (Tailwind, Dark Mode, Renderizado óptimo de imágenes Base64).
7.  **Fase 7: QA Automation** (Pruebas unitarias, de integración y E2E).

---

## 🚀 4. PROMPTS POR MÓDULO

### ### Módulo 1: Database & Data Layer (SQL Server / EF Core)
**Prompt de Ejecución:**
"Desarrolla la capa de acceso a datos para el E-Commerce asegurando que carritos, pasarelas e imágenes residan aquí.
- Utiliza Entity Framework Core con enfoque Code-First[cite: 2].
- Implementa convenciones de nombres donde las tablas estén en plural y PascalCase (ej. `Productos`, `ProductImages`, `ShoppingCarts`)[cite: 4].
- La clave primaria de todas las tablas debe ser estrictamente `Id`[cite: 4].
- Configura las entidades utilizando `IEntityTypeConfiguration<T>` de forma separada[cite: 2].
- Evita totalmente el uso de `NVARCHAR(MAX)`[cite: 4] **excepto** para el caso especial de la tabla de imágenes donde usarás `VARBINARY(MAX)` para la columna `ImageData` que albergará la artesanía.
- Emplea `UNIQUEIDENTIFIER` para las claves primarias[cite: 4] y `DATETIME2` para fechas[cite: 4].
- Incluye soporte obligatorio para Soft Delete mediante una columna `IsDeleted` de tipo BIT[cite: 4]."

### ### Módulo 2: Infrastructure & Core Backend (.NET 8+)
**Prompt de Ejecución:**
"Construye la arquitectura subyacente del backend en .NET 8+.
- Implementa estrictamente Clean Architecture con capas Domain, Application, Infrastructure y Api[cite: 2].
- Todos los repositorios y servicios deben registrarse mediante Inyección de Dependencias[cite: 2].
- Oculta el acceso directo al `DbContext` usando el patrón Repository y Unit of Work[cite: 2].
- Implementa la lógica para convertir `byte[]` de la base de datos a un formato `Base64` en los DTOs de salida para las imágenes de los productos.
- Garantiza propiedades ACID para la transacción de pagos: El registro en `PaymentTransactions` (estado, referencia) debe actualizarse junto con la resta del Stock, vaciado de la tabla `ShoppingCarts` y generación de la Factura en una sola transacción SQL."

### ### Módulo 3: Business API (C#)
**Prompt de Ejecución:**
"Genera los Controladores REST para exponer la lógica de negocio.
- Los endpoints deben retornar `IActionResult` con los verbos HTTP adecuados[cite: 2].
- Las rutas deben ser sustantivos pluralizados en minúsculas y separados por guiones[cite: 5].
- Envuelve las respuestas en un patrón genérico `Result<T>` que contenga `Success`, `Message`, `Data`, y `Errors`[cite: 2].
- Inyecta validaciones en el pipeline usando FluentValidation[cite: 2].
- Modifica el controlador del carrito para que extraiga un header `X-Session-Id` y realice consultas directamente a la tabla `ShoppingCarts` en SQL, sin importar si el usuario está logueado o no."

### ### Módulo 4: Frontend Core & State (React/TS)
**Prompt de Ejecución:**
"Construye la base funcional del portal e-commerce en React.
- Escribe TypeScript estricto, creando `interfaces` o `types` para todo; está terminantemente prohibido usar `any`[cite: 1].
- Define que los tipos del frontend sean un espejo de los DTOs en C#, incluyendo el manejo de imágenes como `string` (Base64)[cite: 1].
- Usa únicamente TanStack Query (React Query) para el manejo del estado del servidor (fetches/mutaciones de productos y del carrito remoto en SQL)[cite: 1].
- Usa Zustand para manejar la generación y almacenamiento inicial del `SessionId` que se enviará en cada petición al backend para gestionar el carrito en SQL[cite: 1]."

### ### Módulo 5: UI/UX & Componentes Visuales
**Prompt de Ejecución:**
"Implementa la interfaz gráfica para el catálogo de artesanías y el flujo de carrito.
- Utiliza Tailwind CSS como framework[cite: 1].
- Configura Tailwind con `darkMode: 'class'` y asegúrate de que todos los componentes web estén diseñados considerando el Modo Oscuro por defecto[cite: 1].
- Separa la arquitectura de interfaz en componentes atómicos en `/src/components/ui` y lógicos en `/src/components/features`[cite: 1].
- Asegúrate de implementar etiquetas `<img src={\`data:image/jpeg;base64,\${producto.imageData}\`} />` para renderizar correctamente el binario traído de SQL."

### ### Módulo 6: Security & Cross-Cutting
**Prompt de Ejecución:**
"Asegura la aplicación y maneja aspectos transversales.
- Configura JWT definiendo Issuer, Audience y Expiration[cite: 5].
- Define Roles y protege endpoints críticos de administración de productos y reportes de pagos usando `[Authorize(Roles = "...")]`[cite: 5].
- Desarrolla un Middleware de Excepciones global que retorne errores bajo el formato `ProblemDetails` estándar (RFC 7807)[cite: 5]."

### ### Módulo 7: QA & Testing
**Prompt de Ejecución:**
"Crea el ecosistema de pruebas automatizadas.
- En el backend, usa `xUnit` junto con `Moq` y `FluentAssertions` para probar la capa de Application, con foco especial en la transacción que vacía el carrito SQL y graba el pago[cite: 3].
- En el frontend, utiliza `Jest` y `React Testing Library` para probar renderizado de componentes críticos y flujos de hooks[cite: 3].
- Genera colecciones en archivos `.http` en la raíz del proyecto para pruebas rápidas de los endpoints[cite: 3]."

---

## 📂 5. ESTRUCTURA DE ARCHIVOS

La aplicación debe prepararse para alojarse bajo la organización de GitHub **Los malditos de diseño de software**[cite: 3], empleando ramas como `main`, `develop` y `feature/nombre-corto` bajo el flujo GitHub Flow[cite: 3].

~~~text
📦 e-commerce-artesanias
 ┣ 📂 backend
 ┃ ┣ 📂 src
 ┃ ┃ ┣ 📂 Artesanias.Domain
 ┃ ┃ ┣ 📂 Artesanias.Application
 ┃ ┃ ┣ 📂 Artesanias.Infrastructure
 ┃ ┃ ┗ 📂 Artesanias.Api
 ┃ ┣ 📜 Artesanias.sln
 ┃ ┗ 📜 appsettings.json
 ┣ 📂 frontend
 ┃ ┣ 📂 src
 ┃ ┃ ┣ 📂 api
 ┃ ┃ ┣ 📂 components
 ┃ ┃ ┃ ┣ 📂 ui
 ┃ ┃ ┃ ┗ 📂 features
 ┃ ┃ ┣ 📂 hooks
 ┃ ┃ ┣ 📂 layouts
 ┃ ┃ ┣ 📂 pages
 ┃ ┃ ┣ 📂 store
 ┃ ┃ ┗ 📂 types
 ┃ ┣ 📜 package.json
 ┃ ┗ 📜 .env.example
 ┗ 📜 .gitignore
~~~
*Nota: Nunca "quemar" credenciales en el código; emplear `appsettings.json` y `.env.example` obligatoriamente[cite: 3].*

---

## ✅ 6. CHECKLIST DE VALIDACIÓN FINAL

*   [ ] ¿Se ha centralizado el almacenamiento de las imágenes de las artesanías utilizando `VARBINARY(MAX)` directamente en SQL Server mediante la tabla `ProductImages`?
*   [ ] ¿El carrito de compras y todos sus ítems se persisten exclusivamente en SQL Server (`ShoppingCarts`) a través de un `SessionId` delegado por el frontend para usuarios anónimos?
*   [ ] ¿Todo el flujo, estado y payload del webhook de la pasarela de pagos se auditan y almacenan en la tabla `PaymentTransactions`?
*   [ ] ¿La estructura de tipos en `/src/types` mapea 1:1 las entidades detalladas en el `INTEGRATION_MANIFEST` (Ts vs C# vs Sql)?
*   [ ] ¿Se configuró el soporte obligatorio de Modo Oscuro con clases de Tailwind[cite: 1]?
*   [ ] ¿Los commits seguirán el estándar Conventional Commits de forma estricta[cite: 3]?