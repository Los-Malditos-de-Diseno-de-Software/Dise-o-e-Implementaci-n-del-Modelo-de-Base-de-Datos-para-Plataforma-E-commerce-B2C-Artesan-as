using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Artesanias.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Artesanos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR(150)", maxLength: 150, nullable: false),
                    HistoriaBiografia = table.Column<string>(type: "NVARCHAR(2000)", maxLength: 2000, nullable: false),
                    ComunidadOrigen = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    CreatedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    IsDeleted = table.Column<bool>(type: "BIT", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artesanos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    Apellido = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR(200)", maxLength: 200, nullable: false),
                    PasswordHash = table.Column<string>(type: "NVARCHAR(256)", maxLength: 256, nullable: false),
                    Rol = table.Column<string>(type: "NVARCHAR(50)", maxLength: 50, nullable: false),
                    Telefono = table.Column<string>(type: "NVARCHAR(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    CreatedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    IsDeleted = table.Column<bool>(type: "BIT", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false),
                    ArtesanoId = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "NVARCHAR(1000)", maxLength: 1000, nullable: false),
                    Precio = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false),
                    Stock = table.Column<int>(type: "INT", nullable: false),
                    EsUnico = table.Column<bool>(type: "BIT", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    CreatedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    IsDeleted = table.Column<bool>(type: "BIT", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Productos_Artesanos_ArtesanoId",
                        column: x => x.ArtesanoId,
                        principalTable: "Artesanos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false),
                    Total = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false),
                    EstadoPedido = table.Column<string>(type: "NVARCHAR(50)", maxLength: 50, nullable: false),
                    DireccionEnvio = table.Column<string>(type: "NVARCHAR(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    CreatedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    IsDeleted = table.Column<bool>(type: "BIT", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShoppingCarts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false),
                    SessionId = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: true),
                    UltimaActualizacion = table.Column<DateTime>(type: "DATETIME2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCarts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShoppingCarts_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ProductImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false),
                    ProductoId = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false),
                    ImageData = table.Column<byte[]>(type: "VARBINARY(MAX)", nullable: false),
                    ContentType = table.Column<string>(type: "NVARCHAR(50)", maxLength: 50, nullable: false),
                    EsPrincipal = table.Column<bool>(type: "BIT", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "DATETIME2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductImages_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false),
                    OrderId = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false),
                    ProductoId = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false),
                    Cantidad = table.Column<int>(type: "INT", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false),
                    OrderId = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false),
                    MetodoPago = table.Column<string>(type: "NVARCHAR(50)", maxLength: 50, nullable: false),
                    EstadoPago = table.Column<string>(type: "NVARCHAR(50)", maxLength: 50, nullable: false),
                    ReferenciaPasarela = table.Column<string>(type: "NVARCHAR(200)", maxLength: 200, nullable: false),
                    PayloadPasarela = table.Column<string>(type: "NVARCHAR(4000)", maxLength: 4000, nullable: false),
                    StripeSessionId = table.Column<string>(type: "NVARCHAR(200)", maxLength: 200, nullable: false),
                    StripePaymentIntentId = table.Column<string>(type: "NVARCHAR(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "DATETIME2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentTransactions_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false),
                    ShoppingCartId = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false),
                    ProductoId = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false),
                    Cantidad = table.Column<int>(type: "INT", nullable: false),
                    PrecioUnitarioCongelado = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartItems_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CartItems_ShoppingCarts_ShoppingCartId",
                        column: x => x.ShoppingCartId,
                        principalTable: "ShoppingCarts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ProductoId",
                table: "CartItems",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ShoppingCartId",
                table: "CartItems",
                column: "ShoppingCartId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductoId",
                table: "OrderItems",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UsuarioId",
                table: "Orders",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_OrderId",
                table: "PaymentTransactions",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_StripeSessionId",
                table: "PaymentTransactions",
                column: "StripeSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductoId",
                table: "ProductImages",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_ArtesanoId",
                table: "Productos",
                column: "ArtesanoId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCarts_SessionId",
                table: "ShoppingCarts",
                column: "SessionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCarts_UsuarioId",
                table: "ShoppingCarts",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "PaymentTransactions");

            migrationBuilder.DropTable(
                name: "ProductImages");

            migrationBuilder.DropTable(
                name: "ShoppingCarts");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Productos");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Artesanos");
        }
    }
}
