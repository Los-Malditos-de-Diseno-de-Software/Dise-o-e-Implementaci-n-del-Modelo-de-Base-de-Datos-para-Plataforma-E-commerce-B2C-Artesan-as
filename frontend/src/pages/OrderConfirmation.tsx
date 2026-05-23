import { CheckCircle, Home, ArrowRight } from 'lucide-react';
import { useSearchParams, Link } from 'react-router-dom';

export const OrderConfirmation = () => {
  const [searchParams] = useSearchParams();
  const sessionId = searchParams.get('session_id') || '';

  return (
    <div style={{ textAlign: 'center', padding: '5rem 0', maxWidth: '600px', margin: '0 auto' }}>
      {/* Animated Glowing Success Badge */}
      <div style={{
        display: 'inline-flex',
        alignItems: 'center',
        justifyContent: 'center',
        width: '80px',
        height: '80px',
        borderRadius: '50%',
        backgroundColor: 'rgba(16, 185, 129, 0.1)',
        color: 'var(--success-color)',
        marginBottom: '2rem',
        boxShadow: '0 0 30px rgba(16, 185, 129, 0.3)',
        animation: 'pulse 2s infinite',
      }}>
        <CheckCircle size={44} />
      </div>

      <h1 className="text-gradient" style={{ fontSize: '2.75rem', marginBottom: '1.5rem' }}>
        ¡Pago Confirmado!
      </h1>
      <p style={{ fontSize: '1.2rem', color: 'var(--text-primary)', marginBottom: '1rem' }}>
        Gracias por tu compra. Tu pedido ha sido procesado de forma exitosa.
      </p>
      <p style={{ color: 'var(--text-secondary)', marginBottom: '3rem', fontSize: '0.95rem', lineHeight: '1.6' }}>
        Los artesanos cusqueños ya están preparando tu pieza única. El stock ha sido reservado y el carrito de compras ha sido liberado para tu próxima compra.
      </p>

      {sessionId && (
        <div className="glass-panel" style={{
          padding: '1rem',
          backgroundColor: 'rgba(0,0,0,0.2)',
          border: '1px solid var(--border-color)',
          fontSize: '0.85rem',
          color: 'var(--text-secondary)',
          fontFamily: 'monospace',
          marginBottom: '3rem',
          wordBreak: 'break-all'
        }}>
          ID de Sesión de Stripe: {sessionId}
        </div>
      )}

      <div style={{ display: 'flex', gap: '1rem', justifyContent: 'center' }}>
        <Link to="/" className="btn btn-outline" style={{ padding: '0.8rem 1.5rem' }}>
          <Home size={18} />
          <span>Volver al Inicio</span>
        </Link>
        <Link to="/catalogo" className="btn btn-primary" style={{ padding: '0.8rem 1.5rem' }}>
          <span>Seguir comprando</span>
          <ArrowRight size={18} />
        </Link>
      </div>

      <style>{`
        @keyframes pulse {
          0% { transform: scale(1); box-shadow: 0 0 30px rgba(16, 185, 129, 0.3); }
          50% { transform: scale(1.05); box-shadow: 0 0 45px rgba(16, 185, 129, 0.5); }
          100% { transform: scale(1); box-shadow: 0 0 30px rgba(16, 185, 129, 0.3); }
        }
      `}</style>
    </div>
  );
};
