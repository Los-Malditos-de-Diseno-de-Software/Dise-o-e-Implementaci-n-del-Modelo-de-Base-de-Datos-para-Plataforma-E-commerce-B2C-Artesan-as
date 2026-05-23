import { Link } from 'react-router-dom';

export const Home = () => {
  return (
    <div style={{ textAlign: 'center', padding: '4rem 0' }}>
      <h1 style={{ fontSize: '3.5rem', marginBottom: '1.5rem' }}>
        Descubre la magia de las <span className="text-gradient">Artesanías Cusqueñas</span>
      </h1>
      <p style={{ fontSize: '1.25rem', color: 'var(--text-secondary)', maxWidth: '800px', margin: '0 auto 3rem' }}>
        Piezas únicas hechas a mano por maestros artesanos, llevando la cultura incaica a tu hogar.
      </p>
      
      <div style={{ display: 'flex', gap: '1rem', justifyContent: 'center' }}>
        <Link to="/catalogo" className="btn btn-primary" style={{ padding: '1rem 2rem', fontSize: '1.125rem' }}>
          Ver Catálogo
        </Link>
      </div>

      <div className="grid-cards" style={{ marginTop: '5rem' }}>
        {/* Aquí irían componentes de productos destacados */}
        <div className="glass-panel" style={{ padding: '2rem' }}>
          <h3>Arte Textil</h3>
          <p style={{ color: 'var(--text-secondary)' }}>Tejidos a mano con lana de alpaca.</p>
        </div>
        <div className="glass-panel" style={{ padding: '2rem' }}>
          <h3>Cerámica</h3>
          <p style={{ color: 'var(--text-secondary)' }}>Piezas utilitarias y decorativas.</p>
        </div>
        <div className="glass-panel" style={{ padding: '2rem' }}>
          <h3>Joyería</h3>
          <p style={{ color: 'var(--text-secondary)' }}>Plata peruana con incrustaciones.</p>
        </div>
      </div>
    </div>
  );
};
