import React from 'react';
import { X, Trash2, ShoppingBag, Plus, Minus, ArrowRight } from 'lucide-react';
import { useNavigate } from 'react-router-dom';
import { useUIStore } from '../../store/uiStore';
import { useCart } from '../../hooks/useCart';

export const CartDrawer: React.FC = () => {
  const { isCartOpen, closeCart } = useUIStore();
  const { cart, isLoading, removeItem, addItem } = useCart();
  const navigate = useNavigate();

  if (!isCartOpen) return null;

  const handleCheckout = () => {
    closeCart();
    navigate('/checkout');
  };

  const handleIncrement = async (productoId: string) => {
    try {
      await addItem({ productoId, cantidad: 1 });
    } catch (e) {
      console.error(e);
    }
  };

  const handleDecrement = async (itemId: string, currentQty: number, productoId: string) => {
    try {
      if (currentQty > 1) {
        await addItem({ productoId, cantidad: -1 });
      } else {
        await removeItem(itemId);
      }
    } catch (e) {
      console.error(e);
    }
  };

  const formattedTotal = new Intl.NumberFormat('es-PE', {
    style: 'currency',
    currency: 'PEN',
  }).format(cart?.total || 0);

  const items = cart?.items || [];

  return (
    <div style={{
      position: 'fixed',
      top: 0,
      left: 0,
      right: 0,
      bottom: 0,
      zIndex: 1000,
      display: 'flex',
      justifyContent: 'flex-end',
    }}>
      {/* Backdrop */}
      <div 
        onClick={closeCart}
        style={{
          position: 'absolute',
          top: 0,
          left: 0,
          right: 0,
          bottom: 0,
          background: 'rgba(15, 23, 42, 0.6)',
          backdropFilter: 'blur(4px)',
          animation: 'fadeIn 0.2s ease-out',
        }} 
      />

      {/* Drawer Panel */}
      <div style={{
        position: 'relative',
        width: '100%',
        maxWidth: '450px',
        height: '100%',
        background: 'var(--bg-surface)',
        borderLeft: '1px solid var(--border-color)',
        boxShadow: '-10px 0 30px rgba(0,0,0,0.5)',
        display: 'flex',
        flexDirection: 'column',
        animation: 'slideIn 0.3s cubic-bezier(0.16, 1, 0.3, 1)',
      }}>
        {/* Header */}
        <div style={{
          padding: '1.5rem',
          borderBottom: '1px solid var(--border-color)',
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'space-between',
        }}>
          <div style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
            <ShoppingBag size={20} color="var(--accent-color)" />
            <h2 style={{ fontSize: '1.25rem', margin: 0 }}>Tu Carrito</h2>
            <span style={{
              fontSize: '0.75rem',
              backgroundColor: 'rgba(59, 130, 246, 0.1)',
              color: 'var(--accent-color)',
              padding: '2px 8px',
              borderRadius: '20px',
              fontWeight: '600'
            }}>
              {items.length} {items.length === 1 ? 'ítem' : 'ítems'}
            </span>
          </div>
          <button 
            onClick={closeCart}
            style={{
              padding: '4px',
              borderRadius: '50%',
              backgroundColor: 'rgba(255,255,255,0.05)',
              color: 'var(--text-secondary)',
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
              transition: 'all 0.2s',
            }}
            onMouseEnter={(e) => e.currentTarget.style.color = 'white'}
            onMouseLeave={(e) => e.currentTarget.style.color = 'var(--text-secondary)'}
          >
            <X size={20} />
          </button>
        </div>

        {/* Cart items */}
        <div style={{
          flex: 1,
          overflowY: 'auto',
          padding: '1.5rem',
          display: 'flex',
          flexDirection: 'column',
          gap: '1rem',
        }}>
          {isLoading ? (
            <div style={{ display: 'flex', flex: 1, alignItems: 'center', justifyContent: 'center' }}>
              <div className="spinner-mini" style={{ width: '32px', height: '32px' }} />
            </div>
          ) : items.length === 0 ? (
            <div style={{
              display: 'flex',
              flexDirection: 'column',
              alignItems: 'center',
              justifyContent: 'center',
              flex: 1,
              color: 'var(--text-secondary)',
              textAlign: 'center',
              padding: '2rem',
            }}>
              <ShoppingBag size={48} style={{ marginBottom: '1rem', opacity: 0.5 }} />
              <h3>El carrito está vacío</h3>
              <p style={{ fontSize: '0.875rem', marginTop: '0.5rem' }}>
                ¡Explora nuestra colección y añade algunas obras únicas de Cusco!
              </p>
              <button 
                onClick={closeCart}
                className="btn btn-outline" 
                style={{ marginTop: '1.5rem', width: '100%' }}
              >
                Volver a la Tienda
              </button>
            </div>
          ) : (
            items.map((item) => (
              <div 
                key={item.id}
                className="glass-panel"
                style={{
                  display: 'flex',
                  gap: '1rem',
                  padding: '1rem',
                  alignItems: 'center',
                  background: 'rgba(0,0,0,0.15)',
                  position: 'relative'
                }}
              >
                {/* Image */}
                <div style={{
                  width: '70px',
                  height: '70px',
                  borderRadius: 'var(--radius-sm)',
                  backgroundColor: 'rgba(0,0,0,0.2)',
                  overflow: 'hidden',
                  flexShrink: 0,
                }}>
                  {(() => {
                    const imagen = item.productoImagenBase64 || item.imagenBase64;
                    return imagen ? (
                      <img 
                        src={imagen.startsWith('data:') ? imagen : `data:image/jpeg;base64,${imagen}`}
                        alt={item.productoNombre}
                        style={{ width: '100%', height: '100%', objectFit: 'cover' }}
                      />
                    ) : (
                      <div style={{ width: '100%', height: '100%', display: 'flex', alignItems: 'center', justifyContent: 'center', background: '#1e293b', color: 'rgba(255,255,255,0.3)' }} />
                    );
                  })()}
                </div>

                {/* Details */}
                <div style={{ flex: 1, display: 'flex', flexDirection: 'column', gap: '4px' }}>
                  <h4 style={{ fontSize: '0.95rem', margin: 0, paddingRight: '1.5rem', display: '-webkit-box', WebkitLineClamp: 1, WebkitBoxOrient: 'vertical', overflow: 'hidden' }}>
                    {item.productoNombre}
                  </h4>
                  <span style={{ fontSize: '0.875rem', color: 'var(--text-secondary)' }}>
                    {new Intl.NumberFormat('es-PE', { style: 'currency', currency: 'PEN' }).format(item.precioUnitarioCongelado || item.precioUnitario || 0)}
                  </span>
                  
                  {/* Qty controls */}
                  <div style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', marginTop: '4px' }}>
                    <button 
                      onClick={() => handleDecrement(item.id, item.cantidad, item.productoId)}
                      style={{ padding: '2px', borderRadius: '4px', background: 'rgba(255,255,255,0.05)', color: 'white', display: 'flex' }}
                    >
                      <Minus size={14} />
                    </button>
                    <span style={{ fontSize: '0.875rem', minWidth: '20px', textAlign: 'center', fontWeight: '600' }}>
                      {item.cantidad}
                    </span>
                    <button 
                      onClick={() => handleIncrement(item.productoId)}
                      style={{ padding: '2px', borderRadius: '4px', background: 'rgba(255,255,255,0.05)', color: 'white', display: 'flex' }}
                    >
                      <Plus size={14} />
                    </button>
                  </div>
                </div>

                {/* Delete button */}
                <button 
                  onClick={() => removeItem(item.id)}
                  style={{
                    position: 'absolute',
                    top: '12px',
                    right: '12px',
                    color: 'var(--text-secondary)',
                    transition: 'color 0.2s',
                  }}
                  onMouseEnter={(e) => e.currentTarget.style.color = 'var(--danger-color)'}
                  onMouseLeave={(e) => e.currentTarget.style.color = 'var(--text-secondary)'}
                >
                  <Trash2 size={16} />
                </button>
              </div>
            ))
          )}
        </div>

        {/* Footer Summary */}
        {items.length > 0 && (
          <div style={{
            padding: '1.5rem',
            borderTop: '1px solid var(--border-color)',
            background: 'rgba(15, 23, 42, 0.4)',
            display: 'flex',
            flexDirection: 'column',
            gap: '1rem',
          }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
              <span style={{ color: 'var(--text-secondary)' }}>Total Estimado:</span>
              <span style={{ fontSize: '1.5rem', fontWeight: '700', color: 'var(--text-primary)' }}>{formattedTotal}</span>
            </div>
            
            <button 
              onClick={handleCheckout}
              className="btn btn-primary"
              style={{
                width: '100%',
                padding: '1rem',
                fontSize: '1rem',
                justifyContent: 'center',
                boxShadow: '0 4px 15px rgba(59, 130, 246, 0.3)',
              }}
            >
              <span>Proceder al Pago</span>
              <ArrowRight size={18} />
            </button>
            <p style={{ fontSize: '0.75rem', color: 'var(--text-secondary)', textAlign: 'center', margin: 0 }}>
              Transacción protegida mediante Stripe.
            </p>
          </div>
        )}
      </div>

      <style>{`
        @keyframes fadeIn {
          from { opacity: 0; }
          to { opacity: 1; }
        }
        @keyframes slideIn {
          from { transform: translateX(100%); }
          to { transform: translateX(0); }
        }
        .spinner-mini {
          width: 20px;
          height: 20px;
          border: 2px solid rgba(255, 255, 255, 0.1);
          border-top: 2px solid var(--accent-color);
          border-radius: 50%;
          animation: spin 0.8s linear infinite;
        }
      `}</style>
    </div>
  );
};
