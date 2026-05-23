import { useProductos } from '../../hooks/useProductos';
import { useAdminArtesanos } from '../../hooks/useAdminArtesanos';
import { useAdminOrders } from '../../hooks/useAdminOrders';
import { Spinner } from '../../components/ui/Spinner';
import { Landmark, Package, ShoppingCart, TrendingUp } from 'lucide-react';
import styles from './AdminDashboard.module.css';

export const AdminDashboard = () => {
  const { data: productosResponse, isLoading: loadingProducts } = useProductos({ pageSize: 100 });
  const { artesanos, isLoading: loadingArtisans } = useAdminArtesanos();
  const { orders, isLoading: loadingOrders } = useAdminOrders();

  const isLoading = loadingProducts || loadingArtisans || loadingOrders;

  if (isLoading) {
    return (
      <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '50vh' }}>
        <Spinner size="lg" />
      </div>
    );
  }

  const totalProducts = productosResponse?.total || 0;
  const totalArtisans = artesanos.length;
  const totalOrders = orders.length;

  // Calculate total revenue from confirmed/completed orders
  const totalRevenue = orders
    .filter(order => order.estadoPedido === 'Confirmado' || order.estadoPedido === 'Entregado' || order.pago?.estadoPago === 'Pagado')
    .reduce((sum, order) => sum + order.total, 0);

  const recentOrders = [...orders]
    .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
    .slice(0, 5);

  return (
    <div className={styles.dashboardContainer}>
      <h1 className="text-gradient">Resumen Ejecutivo</h1>
      <p style={{ color: 'var(--text-secondary)', marginBottom: '2rem' }}>
        Monitoreo general de ventas, productos y artesanos de Cusco.
      </p>

      <div className={styles.statsGrid}>
        <div className={`glass-panel ${styles.statCard}`}>
          <div className={styles.statIcon} style={{ background: 'rgba(16, 185, 129, 0.15)', color: 'var(--success-color)' }}>
            <TrendingUp size={24} />
          </div>
          <div className={styles.statInfo}>
            <span className={styles.statLabel}>Ventas Totales</span>
            <span className={styles.statValue}>S/. {totalRevenue.toFixed(2)}</span>
          </div>
        </div>

        <div className={`glass-panel ${styles.statCard}`}>
          <div className={styles.statIcon} style={{ background: 'rgba(59, 130, 246, 0.15)', color: 'var(--accent-color)' }}>
            <ShoppingCart size={24} />
          </div>
          <div className={styles.statInfo}>
            <span className={styles.statLabel}>Pedidos</span>
            <span className={styles.statValue}>{totalOrders}</span>
          </div>
        </div>

        <div className={`glass-panel ${styles.statCard}`}>
          <div className={styles.statIcon} style={{ background: 'rgba(217, 119, 54, 0.15)', color: '#D97736' }}>
            <Package size={24} />
          </div>
          <div className={styles.statInfo}>
            <span className={styles.statLabel}>Obras de Arte</span>
            <span className={styles.statValue}>{totalProducts}</span>
          </div>
        </div>

        <div className={`glass-panel ${styles.statCard}`}>
          <div className={styles.statIcon} style={{ background: 'rgba(139, 92, 246, 0.15)', color: '#8b5cf6' }}>
            <Landmark size={24} />
          </div>
          <div className={styles.statInfo}>
            <span className={styles.statLabel}>Artesanos</span>
            <span className={styles.statValue}>{totalArtisans}</span>
          </div>
        </div>
      </div>

      <div className={styles.sectionsGrid}>
        <div className={`glass-panel ${styles.recentPanel}`}>
          <h3>Pedidos Recientes</h3>
          {recentOrders.length === 0 ? (
            <p className={styles.emptyText}>No hay pedidos registrados en el sistema.</p>
          ) : (
            <div className={styles.tableResponsive}>
              <table className={styles.table}>
                <thead>
                  <tr>
                    <th>Cliente / Envío</th>
                    <th>Fecha</th>
                    <th>Estado Pedido</th>
                    <th>Total</th>
                  </tr>
                </thead>
                <tbody>
                  {recentOrders.map(order => (
                    <tr key={order.id}>
                      <td>
                        <div className={styles.clientCell}>
                          <span className={styles.clientAddress}>{order.direccionEnvio}</span>
                        </div>
                      </td>
                      <td>{new Date(order.createdAt).toLocaleDateString()}</td>
                      <td>
                        <span className={`${styles.badge} ${
                          order.estadoPedido === 'Confirmado' || order.pago?.estadoPago === 'Pagado'
                            ? styles.badgeSuccess
                            : order.estadoPedido === 'Cancelado'
                            ? styles.badgeDanger
                            : styles.badgeWarning
                        }`}>
                          {order.estadoPedido}
                        </span>
                      </td>
                      <td className={styles.totalCell}>S/. {order.total.toFixed(2)}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>

        <div className={`glass-panel ${styles.quickActions}`}>
          <h3>Enlaces Rápidos</h3>
          <div className={styles.actionGrid}>
            <a href="/admin/productos" className={styles.actionBtn}>
              <Package size={18} />
              <span>Gestionar Catálogo</span>
            </a>
            <a href="/admin/artesanos" className={styles.actionBtn}>
              <Landmark size={18} />
              <span>Gestionar Artesanos</span>
            </a>
            <a href="/admin/pedidos" className={styles.actionBtn}>
              <ShoppingCart size={18} />
              <span>Verificar Pedidos</span>
            </a>
          </div>
        </div>
      </div>
    </div>
  );
};
