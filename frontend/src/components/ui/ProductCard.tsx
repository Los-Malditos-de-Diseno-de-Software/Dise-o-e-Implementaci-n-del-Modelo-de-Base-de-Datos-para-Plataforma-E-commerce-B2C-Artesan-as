import React from 'react';
import { ShoppingCart } from 'lucide-react';
import type { ProductoDto } from '../../types';
import { useCart } from '../../hooks/useCart';

interface ProductCardProps {
  producto: ProductoDto;
}

export const ProductCard: React.FC<ProductCardProps> = ({ producto }) => {
  const { addItem, isAdding } = useCart();

  const handleAddToCart = async (e: React.MouseEvent) => {
    e.preventDefault();
    e.stopPropagation();
    try {
      await addItem({ productoId: producto.id, cantidad: 1 });
    } catch (err) {
      console.error('Error adding to cart', err);
    }
  };

  const formattedPrecio = new Intl.NumberFormat('es-PE', {
    style: 'currency',
    currency: 'PEN',
  }).format(producto.precio);

  const hasStock = producto.stock > 0;

  return (
    <div className="glass-panel card-hover" style={{
      display: 'flex',
      flexDirection: 'column',
      overflow: 'hidden',
      position: 'relative',
      height: '100%',
      transition: 'transform var(--transition-fast), box-shadow var(--transition-fast)',
    }}>
      {/* Badges overlay */}
      <div style={{ position: 'absolute', top: '12px', left: '12px', zIndex: 5, display: 'flex', gap: '8px', flexDirection: 'column' }}>
        {producto.esUnico && (
          <span style={{
            fontSize: '0.75rem',
            fontWeight: '600',
            background: 'linear-gradient(135deg, #f59e0b 0%, #d97706 100%)',
            color: 'white',
            padding: '2px 8px',
            borderRadius: 'var(--radius-sm)',
            boxShadow: '0 0 10px rgba(245, 158, 11, 0.4)',
          }}>
            Pieza Única
          </span>
        )}
        {!hasStock && (
          <span style={{
            fontSize: '0.75rem',
            fontWeight: '600',
            backgroundColor: 'var(--danger-color)',
            color: 'white',
            padding: '2px 8px',
            borderRadius: 'var(--radius-sm)',
          }}>
            Agotado
          </span>
        )}
      </div>

      {/* Product Image */}
      <div style={{
        position: 'relative',
        paddingTop: '80%', // 5:4 Aspect Ratio
        background: 'rgba(0,0,0,0.2)',
        overflow: 'hidden',
        borderBottom: '1px solid rgba(255, 255, 255, 0.05)',
      }}>
        {producto.imagenBase64 ? (
          <img
            src={producto.imagenBase64.startsWith('data:') ? producto.imagenBase64 : `data:image/jpeg;base64,${producto.imagenBase64}`}
            alt={producto.nombre}
            style={{
              position: 'absolute',
              top: 0,
              left: 0,
              width: '100%',
              height: '100%',
              objectFit: 'cover',
            }}
          />
        ) : (
          <div style={{
            position: 'absolute',
            top: 0,
            left: 0,
            width: '100%',
            height: '100%',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            background: 'linear-gradient(135deg, #1e293b 0%, #0f172a 100%)',
            color: 'var(--text-secondary)',
          }}>
            <span style={{ fontSize: '0.875rem' }}>Sin imagen</span>
          </div>
        )}
      </div>

      {/* Details */}
      <div style={{ padding: '1.25rem', display: 'flex', flexDirection: 'column', flex: 1 }}>
        <span style={{ fontSize: '0.75rem', color: 'var(--accent-color)', fontWeight: '600', textTransform: 'uppercase', marginBottom: '4px', letterSpacing: '0.05em' }}>
          {producto.artesanoNombre}
        </span>
        <h3 style={{ fontSize: '1.125rem', marginBottom: '8px', display: '-webkit-box', WebkitLineClamp: 1, WebkitBoxOrient: 'vertical', overflow: 'hidden' }}>
          {producto.nombre}
        </h3>
        <p style={{
          fontSize: '0.875rem',
          color: 'var(--text-secondary)',
          marginBottom: '1.25rem',
          flex: 1,
          display: '-webkit-box',
          WebkitLineClamp: 2,
          WebkitBoxOrient: 'vertical',
          overflow: 'hidden',
          lineHeight: '1.4'
        }}>
          {producto.descripcion}
        </p>

        {/* Action Row */}
        <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginTop: 'auto', gap: '12px' }}>
          <div style={{ display: 'flex', flexDirection: 'column' }}>
            <span style={{ fontSize: '0.75rem', color: 'var(--text-secondary)' }}>Precio</span>
            <span style={{ fontSize: '1.25rem', fontWeight: '700', color: 'var(--text-primary)' }}>{formattedPrecio}</span>
          </div>

          <button
            className={`btn ${hasStock ? 'btn-primary' : 'btn-outline'}`}
            style={{ padding: '0.5rem 1rem', fontSize: '0.875rem' }}
            onClick={handleAddToCart}
            disabled={!hasStock || isAdding}
          >
            {isAdding ? (
              <span className="spinner-mini"></span>
            ) : (
              <ShoppingCart size={16} />
            )}
            <span>{hasStock ? 'Agregar' : 'Agotado'}</span>
          </button>
        </div>
      </div>
      <style>{`
        .card-hover:hover {
          transform: translateY(-4px);
          box-shadow: 0 12px 24px -10px rgba(0, 0, 0, 0.4), var(--shadow-glow);
          border-color: rgba(59, 130, 246, 0.3) !important;
        }
        .spinner-mini {
          width: 14px;
          height: 14px;
          border: 2px solid rgba(255,255,255,0.2);
          border-top: 2px solid white;
          border-radius: 50%;
          animation: spin 0.6s linear infinite;
        }
      `}</style>
    </div>
  );
};
