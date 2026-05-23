import { Outlet } from 'react-router-dom';
import { Navbar } from './Navbar';
import { CartDrawer } from '../features/CartDrawer';

export const Layout = () => {
  return (
    <>
      <Navbar />
      <CartDrawer />
      <main className="main-content">
        <div className="container animate-fade-in">
          <Outlet />
        </div>
      </main>
      <footer style={{ borderTop: '1px solid var(--border-color)', padding: '2rem 0', marginTop: 'auto', textAlign: 'center', color: 'var(--text-secondary)' }}>
        <p>© {new Date().getFullYear()} ArtesaníasCusco. Todos los derechos reservados.</p>
      </footer>
    </>
  );
};
