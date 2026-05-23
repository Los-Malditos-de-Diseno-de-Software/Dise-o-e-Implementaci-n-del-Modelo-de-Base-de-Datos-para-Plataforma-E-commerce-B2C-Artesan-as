import { Link, NavLink } from 'react-router-dom';
import { ShoppingBag, User, LogOut } from 'lucide-react';
import { useAuthStore } from '../../store/authStore';
import { useUIStore } from '../../store/uiStore';
import { useCart } from '../../hooks/useCart';
import styles from './Navbar.module.css';

export const Navbar = () => {
  const { isAuthenticated, user, logout } = useAuthStore();
  const { toggleCart } = useUIStore();
  const { cart } = useCart();
  
  const cartItemsCount = cart?.items.reduce((acc, item) => acc + item.cantidad, 0) || 0;

  return (
    <header className={styles.header}>
      <div className={`container ${styles.nav}`}>
        <Link to="/" className={styles.logo}>
          <span className="text-gradient">ArtesaníasCusco</span>
        </Link>

        <nav className={styles.links}>
          <NavLink 
            to="/" 
            className={({ isActive }) => isActive ? `${styles.link} ${styles.active}` : styles.link}
          >
            Inicio
          </NavLink>
          <NavLink 
            to="/catalogo" 
            className={({ isActive }) => isActive ? `${styles.link} ${styles.active}` : styles.link}
          >
            Catálogo
          </NavLink>
        </nav>

        <div className={styles.actions}>
          <button className={styles.cartBtn} onClick={toggleCart} aria-label="Abrir carrito">
            <ShoppingBag size={24} />
            {cartItemsCount > 0 && (
              <span className={styles.badge}>{cartItemsCount}</span>
            )}
          </button>

          {isAuthenticated ? (
            <div style={{ display: 'flex', alignItems: 'center', gap: '1rem' }}>
              <span style={{ fontSize: '0.875rem', color: 'var(--text-secondary)' }}>
                Hola, {user?.nombre}
              </span>
              <button className="btn btn-outline" style={{ padding: '0.5rem' }} onClick={logout}>
                <LogOut size={18} />
              </button>
            </div>
          ) : (
            <Link to="/login" className="btn btn-primary" style={{ padding: '0.5rem 1rem' }}>
              <User size={18} />
              <span>Ingresar</span>
            </Link>
          )}
        </div>
      </div>
    </header>
  );
};
