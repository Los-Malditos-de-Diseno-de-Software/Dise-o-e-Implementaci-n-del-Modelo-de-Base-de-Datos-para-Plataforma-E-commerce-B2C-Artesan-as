import { Navigate, Outlet, NavLink, Link } from 'react-router-dom';
import { useAuthStore } from '../../store/authStore';
import { LayoutDashboard, ShoppingBag, Users, ShoppingCart, ArrowLeft, LogOut } from 'lucide-react';
import styles from './AdminLayout.module.css';

export const AdminLayout = () => {
  const { isAuthenticated, user, logout } = useAuthStore();

  // Protect route: must be authenticated and have the 'Administrador' role
  if (!isAuthenticated || user?.rol !== 'Administrador') {
    return <Navigate to="/login" replace />;
  }

  return (
    <div className={styles.adminContainer}>
      <aside className={styles.sidebar}>
        <div className={styles.sidebarHeader}>
          <Link to="/" className={styles.logo}>
            <span className="text-gradient">ArtesaníasCusco</span>
            <span className={styles.adminBadge}>Admin</span>
          </Link>
        </div>

        <nav className={styles.sidebarNav}>
          <NavLink
            to="/admin"
            end
            className={({ isActive }) => isActive ? `${styles.navItem} ${styles.active}` : styles.navItem}
          >
            <LayoutDashboard size={20} />
            <span>Dashboard</span>
          </NavLink>
          <NavLink
            to="/admin/productos"
            className={({ isActive }) => isActive ? `${styles.navItem} ${styles.active}` : styles.navItem}
          >
            <ShoppingBag size={20} />
            <span>Productos</span>
          </NavLink>
          <NavLink
            to="/admin/artesanos"
            className={({ isActive }) => isActive ? `${styles.navItem} ${styles.active}` : styles.navItem}
          >
            <Users size={20} />
            <span>Artesanos</span>
          </NavLink>
          <NavLink
            to="/admin/pedidos"
            className={({ isActive }) => isActive ? `${styles.navItem} ${styles.active}` : styles.navItem}
          >
            <ShoppingCart size={20} />
            <span>Pedidos</span>
          </NavLink>
        </nav>

        <div className={styles.sidebarFooter}>
          <Link to="/" className={styles.backBtn}>
            <ArrowLeft size={18} />
            <span>Volver a la Tienda</span>
          </Link>
          <button onClick={logout} className={styles.logoutBtn}>
            <LogOut size={18} />
            <span>Cerrar Sesión</span>
          </button>
        </div>
      </aside>

      <div className={styles.mainWrapper}>
        <header className={styles.adminHeader}>
          <div className={styles.headerTitle}>
            <h2>Panel de Control</h2>
          </div>
          <div className={styles.adminProfile}>
            <span>Hola, <strong>{user?.nombre}</strong></span>
            <span className={styles.profileBadge}>{user?.rol}</span>
          </div>
        </header>

        <main className={styles.adminContent}>
          <div className="animate-fade-in">
            <Outlet />
          </div>
        </main>
      </div>
    </div>
  );
};
