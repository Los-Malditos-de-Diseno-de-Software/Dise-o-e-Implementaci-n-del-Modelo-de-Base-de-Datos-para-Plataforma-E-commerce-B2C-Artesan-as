export interface Result<T> {
  success: boolean;
  message?: string;
  errors?: string[];
  data: T;
}

export interface ProductoDto {
  id: string;
  artesanoId: string;
  artesanoNombre: string;
  nombre: string;
  descripcion: string;
  precio: number;
  stock: number;
  esUnico: boolean;
  createdAt: string;
  imagenBase64?: string | null;
}

export interface CartItemDto {
  id: string;
  productoId: string;
  productoNombre: string;
  precioUnitarioCongelado: number;
  precioUnitario?: number;
  cantidad: number;
  subtotal: number;
  productoImagenBase64?: string | null;
  imagenBase64?: string | null;
}

export interface CartDto {
  id: string;
  sessionId: string;
  items: CartItemDto[];
  total: number;
}

export interface LoginRequestDto {
  email: string;
  password?: string;
}

export interface RegisterRequestDto {
  nombre: string;
  apellido: string;
  email: string;
  password?: string;
  telefono?: string;
}

export interface AuthResponseDto {
  id: string;
  token: string;
  expiration: string;
  nombre: string;
  email: string;
  rol: string;
}

export interface ArtesanoDto {
  id: string;
  nombre: string;
  historiaBiografia: string;
  comunidadOrigen: string;
}

export interface OrderItemDto {
  id: string;
  productoId: string;
  productoNombre: string;
  cantidad: number;
  precioUnitario: number;
  subtotal: number;
}

export interface PaymentTransactionDto {
  id: string;
  metodoPago: string;
  estadoPago: string;
  stripeSessionId: string;
  createdAt: string;
}

export interface OrderDto {
  id: string;
  usuarioId: string;
  total: number;
  estadoPedido: string;
  direccionEnvio: string;
  createdAt: string;
  items: OrderItemDto[];
  pago?: PaymentTransactionDto | null;
}

export interface CreateOrderResponseDto {
  orderId: string;
  stripeCheckoutUrl: string;
  stripeSessionId: string;
}
