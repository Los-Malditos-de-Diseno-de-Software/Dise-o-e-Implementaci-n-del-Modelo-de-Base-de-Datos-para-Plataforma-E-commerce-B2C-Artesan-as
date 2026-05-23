import React, { useState } from 'react';
import { ShoppingBag, MapPin, ArrowRight, CreditCard, ChevronLeft } from 'lucide-react';
import { Link } from 'react-router-dom';
import { useCart } from '../hooks/useCart';
import { useCreateOrder } from '../hooks/useCreateOrder';
import { useAuthStore } from '../store/authStore';
import { Spinner } from '../components/ui/Spinner';

// Helper to decode JWT token and extract the "sub" claim (user ID)
const getUsuarioIdFromToken = (token: string | null): string | null => {
  if (!token) return null;
  try {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(
      window
        .atob(base64)
        .split('')
        .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
        .join('')
    );
    const parsed = JSON.parse(jsonPayload);
    return parsed.sub || null;
  } catch (e) {
    console.error('Error decoding token:', e);
    return null;
  }
};

export const Checkout = () => {
  const { cart, isLoading: isCartLoading } = useCart();
  const { mutateAsync: createOrder, isPending: isPlacingOrder } = useCreateOrder();
  const { user, token } = useAuthStore();
  
  const [direccion, setDireccion] = useState('');
  const [errorMsg, setErrorMsg] = useState('');

  const items = cart?.items || [];

  if (isCartLoading) {
    return (
      <div style={{ padding: '5rem 0' }}>
        <Spinner size="lg" />
        <p style={{ textAlign: 'center', color: 'var(--text-secondary)' }}>Cargando resumen...</p>
      </div>
    );
  }

  if (items.length === 0) {
    return (
      <div style={{ textAlign: 'center', padding: '5rem 0' }}>
        <ShoppingBag size={48} style={{ opacity: 0.5, marginBottom: '1rem' }} />
        <h2>Tu carrito está vacío</h2>
        <p style={{ color: 'var(--text-secondary)', marginTop: '0.5rem' }}>
          Añade artículos al carrito para poder proceder con el pago.
        </p>
        <Link to="/catalogo" className="btn btn-primary" style={{ marginTop: '1.5rem' }}>
          Ir al catálogo
        </Link>
      </div>
    );
  }

  const handleCheckoutSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!direccion.trim()) {
      setErrorMsg('Por favor, ingresa una dirección de envío válida.');
      return;
    }
    
    // Decodificar el ID con redundancia por si la sesión está desactualizada
    const usuarioId = user?.id || (user as any)?.Id || getUsuarioIdFromToken(token);
    
    if (!usuarioId) {
      setErrorMsg('Debes iniciar sesión para realizar una compra.');
      return;
    }
    setErrorMsg('');
    try {
      await createOrder({ direccionEnvio: direccion, usuarioId });
    } catch (err: any) {
      console.error(err);
      setErrorMsg(err?.response?.data?.message || 'Error al procesar la orden. Inténtalo de nuevo.');
    }
  };

  const formattedTotal = new Intl.NumberFormat('es-PE', {
    style: 'currency',
    currency: 'PEN',
  }).format(cart?.total || 0);

  return (
    <div style={{ padding: '2rem 0' }}>
      {/* Return button */}
      <Link to="/catalogo" style={{ display: 'inline-flex', alignItems: 'center', gap: '0.5rem', marginBottom: '2rem', color: 'var(--text-secondary)' }}>
        <ChevronLeft size={16} />
        <span>Volver a la tienda</span>
      </Link>

      <h1 className="text-gradient" style={{ fontSize: '2.5rem', marginBottom: '2rem' }}>Finalizar Compra</h1>

      <div style={{ display: 'grid', gridTemplateColumns: '1fr', gap: '2rem' }} className="grid-checkout">
        {/* Left Form Panel */}
        <form onSubmit={handleCheckoutSubmit} className="glass-panel" style={{ padding: '2rem', border: '1px solid rgba(255,255,255,0.08)' }}>
          <h2 style={{ fontSize: '1.25rem', marginBottom: '1.5rem', display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
            <MapPin size={20} color="var(--accent-color)" />
            <span>Datos de Entrega</span>
          </h2>

          {errorMsg && (
            <div style={{
              backgroundColor: 'rgba(239, 68, 68, 0.1)',
              border: '1px solid var(--danger-color)',
              color: 'white',
              padding: '1rem',
              borderRadius: 'var(--radius-md)',
              marginBottom: '1.5rem',
              fontSize: '0.875rem'
            }}>
              {errorMsg}
            </div>
          )}

          <div className="form-group">
            <label className="form-label">Dirección de Envío Completa</label>
            <input
              type="text"
              required
              placeholder="Ej. Av. El Sol 123, Cusco, Perú"
              value={direccion}
              onChange={(e) => setDireccion(e.target.value)}
              className="form-input"
              disabled={isPlacingOrder}
            />
            <span style={{ fontSize: '0.75rem', color: 'var(--text-secondary)', marginTop: '4px', display: 'block' }}>
              Incluye referencias de casa/departamento, ciudad y departamento.
            </span>
          </div>

          <div style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', margin: '2rem 0', color: 'var(--text-secondary)', fontSize: '0.875rem' }}>
            <CreditCard size={18} />
            <span>Los pagos se procesan de forma 100% segura mediante <strong>Stripe Checkout</strong>.</span>
          </div>

          <button
            type="submit"
            className="btn btn-primary"
            style={{
              width: '100%',
              padding: '1rem',
              fontSize: '1.1rem',
              justifyContent: 'center',
              boxShadow: '0 4px 15px rgba(59, 130, 246, 0.3)',
            }}
            disabled={isPlacingOrder}
          >
            {isPlacingOrder ? (
              <>
                <div className="spinner-mini" />
                <span>Generando Checkout de Stripe...</span>
              </>
            ) : (
              <>
                <span>Proceder al pago con Stripe</span>
                <ArrowRight size={18} />
              </>
            )}
          </button>
        </form>

        {/* Right Cart Summary Panel */}
        <div className="glass-panel" style={{ padding: '2rem', height: 'fit-content', border: '1px solid rgba(255,255,255,0.08)' }}>
          <h2 style={{ fontSize: '1.25rem', marginBottom: '1.5rem', display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
            <ShoppingBag size={20} color="var(--accent-color)" />
            <span>Resumen del Pedido</span>
          </h2>

          <div style={{ display: 'flex', flexDirection: 'column', gap: '1rem', marginBottom: '1.5rem' }}>
            {items.map((item) => (
              <div key={item.id} style={{ display: 'flex', gap: '1rem', alignItems: 'center', borderBottom: '1px solid var(--border-color)', paddingBottom: '1rem' }}>
                <div style={{ width: '50px', height: '50px', borderRadius: '4px', overflow: 'hidden', flexShrink: 0 }}>
                  {(() => {
                    const imagen = item.productoImagenBase64 || item.imagenBase64;
                    return imagen ? (
                      <img
                        src={imagen.startsWith('data:') ? imagen : `data:image/jpeg;base64,${imagen}`}
                        alt={item.productoNombre}
                        style={{ width: '100%', height: '100%', objectFit: 'cover' }}
                      />
                    ) : (
                      <div style={{ width: '100%', height: '100%', backgroundColor: '#1e293b' }} />
                    );
                  })()}
                </div>
                <div style={{ flex: 1 }}>
                  <h4 style={{ fontSize: '0.9rem', margin: 0 }}>{item.productoNombre}</h4>
                  <span style={{ fontSize: '0.75rem', color: 'var(--text-secondary)' }}>Cantidad: {item.cantidad}</span>
                </div>
                <span style={{ fontSize: '0.9rem', fontWeight: '600' }}>
                  {new Intl.NumberFormat('es-PE', { style: 'currency', currency: 'PEN' }).format(item.subtotal)}
                </span>
              </div>
            ))}
          </div>

          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', paddingTop: '1rem' }}>
            <span style={{ fontWeight: '600' }}>Total a pagar:</span>
            <span style={{ fontSize: '1.5rem', fontWeight: '700', color: 'var(--text-primary)' }}>{formattedTotal}</span>
          </div>
        </div>
      </div>
      <style>{`
        @media(min-width: 900px) {
          .grid-checkout {
            grid-template-columns: 1.2fr 0.8fr;
          }
        }
        .spinner-mini {
          width: 18px;
          height: 18px;
          border: 2px solid rgba(255, 255, 255, 0.2);
          border-top: 2px solid white;
          border-radius: 50%;
          animation: spin 0.6s linear infinite;
          margin-right: 8px;
        }
      `}</style>
    </div>
  );
};
