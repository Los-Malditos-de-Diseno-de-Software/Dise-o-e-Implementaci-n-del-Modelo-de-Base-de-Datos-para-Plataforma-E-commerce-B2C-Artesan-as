import { Routes, Route } from 'react-router-dom';
import { Layout } from './components/layout/Layout';
import { AdminLayout } from './components/layout/AdminLayout';
import { Home } from './pages/Home';
import { Catalogo } from './pages/Catalogo';
import { Checkout } from './pages/Checkout';
import { OrderConfirmation } from './pages/OrderConfirmation';
import { Login } from './pages/Login';
import { Register } from './pages/Register';

// Admin Pages
import { AdminDashboard } from './pages/admin/AdminDashboard';
import { AdminProductos } from './pages/admin/AdminProductos';
import { AdminArtesanos } from './pages/admin/AdminArtesanos';
import { AdminPedidos } from './pages/admin/AdminPedidos';

function App() {
  return (
    <Routes>
      {/* Public Routes */}
      <Route path="/" element={<Layout />}>
        <Route index element={<Home />} />
        <Route path="catalogo" element={<Catalogo />} />
        <Route path="checkout" element={<Checkout />} />
        <Route path="confirmacion" element={<OrderConfirmation />} />
        <Route path="login" element={<Login />} />
        <Route path="registro" element={<Register />} />
      </Route>

      {/* Admin Protected Routes */}
      <Route path="/admin" element={<AdminLayout />}>
        <Route index element={<AdminDashboard />} />
        <Route path="productos" element={<AdminProductos />} />
        <Route path="artesanos" element={<AdminArtesanos />} />
        <Route path="pedidos" element={<AdminPedidos />} />
      </Route>
    </Routes>
  );
}

export default App;
