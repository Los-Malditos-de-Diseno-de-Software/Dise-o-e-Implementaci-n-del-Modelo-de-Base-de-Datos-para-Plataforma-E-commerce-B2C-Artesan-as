import { useState } from 'react';
import { useAdminOrders } from '../../hooks/useAdminOrders';
import { Spinner } from '../../components/ui/Spinner';
import { ShoppingBag, Eye, Calendar, MapPin, DollarSign, X } from 'lucide-react';
import styles from './AdminPedidos.module.css';

export const AdminPedidos = () => {
  const { orders, isLoading } = useAdminOrders();
  const [selectedOrder, setSelectedOrder] = useState<any | null>(null);

  // Filter orders
  const [filterStatus, setFilterStatus] = useState<string>('Todos');

  const filteredOrders = orders.filter((order: any) => {
    if (filterStatus === 'Todos') return true;
    return order.estadoPedido === filterStatus;
  });

  const getBadgeClass = (status: string) => {
    switch (status) {
      case 'Confirmado':
        return styles.badgeSuccess;
      case 'Cancelado':
        return styles.badgeDanger;
      case 'Entregado':
        return styles.badgeSuccess;
      default:
        return styles.badgeWarning;
    }
  };

  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <div>
          <h1 className="text-gradient">Registro de Transacciones</h1>
          <p style={{ color: 'var(--text-secondary)' }}>
            Revisa, filtra y audita los pedidos de los coleccionistas y amantes del arte andino.
          </p>
        </div>

        <div className={styles.filters}>
          <select value={filterStatus} onChange={e => setFilterStatus(e.target.value)} className={styles.selectFilter}>
            <option value="Todos">Todos los Estados</option>
            <option value="Pendiente">Pendientes</option>
            <option value="Confirmado">Confirmados</option>
            <option value="Entregado">Entregados</option>
            <option value="Cancelado">Cancelados</option>
          </select>
        </div>
      </div>

      {isLoading ? (
        <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '40vh' }}>
          <Spinner size="lg" />
        </div>
      ) : (
        <div className={`glass-panel ${styles.tablePanel}`}>
          <div className={styles.tableResponsive}>
            <table className={styles.table}>
              <thead>
                <tr>
                  <th>ID Pedido</th>
                  <th>Fecha</th>
                  <th>Dirección de Envío</th>
                  <th>Total</th>
                  <th>Estado Pedido</th>
                  <th>Pago Stripe</th>
                  <th style={{ textAlign: 'right' }}>Detalle</th>
                </tr>
              </thead>
              <tbody>
                {filteredOrders.length === 0 ? (
                  <tr>
                    <td colSpan={7} style={{ textAlign: 'center', padding: '3rem 0', color: 'var(--text-secondary)' }}>
                      No se encontraron pedidos con este filtro.
                    </td>
                  </tr>
                ) : (
                  filteredOrders.map((order: any) => (
                    <tr key={order.id}>
                      <td className={styles.idCell}>#{order.id.substring(0, 8)}</td>
                      <td>
                        <div style={{ display: 'flex', alignItems: 'center', gap: '0.375rem' }}>
                          <Calendar size={14} style={{ color: 'var(--text-secondary)' }} />
                          <span>{new Date(order.createdAt).toLocaleDateString()}</span>
                        </div>
                      </td>
                      <td>
                        <div style={{ display: 'flex', alignItems: 'center', gap: '0.375rem' }}>
                          <MapPin size={14} style={{ color: 'var(--text-secondary)' }} />
                          <span className={styles.addressCell}>{order.direccionEnvio}</span>
                        </div>
                      </td>
                      <td className={styles.priceCell}>S/. {order.total.toFixed(2)}</td>
                      <td>
                        <span className={`${styles.badge} ${getBadgeClass(order.estadoPedido)}`}>
                          {order.estadoPedido}
                        </span>
                      </td>
                      <td>
                        {order.pago ? (
                          <span className={`${styles.pagoBadge} ${order.pago.estadoPago === 'Pagado' ? styles.badgeSuccess : styles.badgeWarning}`}>
                            {order.pago.estadoPago}
                          </span>
                        ) : (
                          <span className={styles.pagoNinguno}>Sin Registro</span>
                        )}
                      </td>
                      <td style={{ textAlign: 'right' }}>
                        <button onClick={() => setSelectedOrder(order)} className={styles.viewBtn}>
                          <Eye size={16} />
                        </button>
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* Detail Modal */}
      {selectedOrder && (
        <div className={styles.modalOverlay}>
          <div className={`glass-panel ${styles.modalContent}`}>
            <div className={styles.modalHeader}>
              <h3>Detalle del Pedido #{selectedOrder.id.substring(0, 8)}</h3>
              <button onClick={() => setSelectedOrder(null)} className={styles.closeBtn}>
                <X size={20} />
              </button>
            </div>

            <div className={styles.detailsGrid}>
              <div className={styles.detailBlock}>
                <h4>
                  <MapPin size={16} />
                  <span>Información de Envío</span>
                </h4>
                <p><strong>Dirección:</strong> {selectedOrder.direccionEnvio}</p>
                <p><strong>Fecha de Orden:</strong> {new Date(selectedOrder.createdAt).toLocaleString()}</p>
              </div>

              <div className={styles.detailBlock}>
                <h4>
                  <DollarSign size={16} />
                  <span>Estado de Pago</span>
                </h4>
                <p>
                  <strong>Pasarela:</strong> {selectedOrder.pago?.metodoPago || 'Stripe Checkout'}
                </p>
                <p>
                  <strong>Estado:</strong>{' '}
                  <span className={`${styles.badge} ${selectedOrder.pago?.estadoPago === 'Pagado' ? styles.badgeSuccess : styles.badgeWarning}`}>
                    {selectedOrder.pago?.estadoPago || 'Pendiente'}
                  </span>
                </p>
                {selectedOrder.pago?.stripeSessionId && (
                  <p className={styles.stripeId}>
                    <strong>ID Sesión Stripe:</strong> {selectedOrder.pago.stripeSessionId.substring(0, 20)}...
                  </p>
                )}
              </div>
            </div>

            <div className={styles.itemsBlock}>
              <h4>
                <ShoppingBag size={16} />
                <span>Productos Comprados</span>
              </h4>
              <div className={styles.itemsList}>
                {selectedOrder.items.map((item: any) => (
                  <div key={item.id} className={styles.itemRow}>
                    <div className={styles.itemNameInfo}>
                      <span className={styles.itemName}>{item.productoNombre}</span>
                      <span className={styles.itemQuant}>x{item.cantidad}</span>
                    </div>
                    <div className={styles.itemPriceInfo}>
                      <span className={styles.itemUnit}>S/. {item.precioUnitario.toFixed(2)} c/u</span>
                      <span className={styles.itemSub}>S/. {item.subtotal.toFixed(2)}</span>
                    </div>
                  </div>
                ))}
              </div>
              <div className={styles.totalRow}>
                <span>Total General</span>
                <span>S/. {selectedOrder.total.toFixed(2)}</span>
              </div>
            </div>

            <div className={styles.modalActions}>
              <button onClick={() => setSelectedOrder(null)} className="btn btn-primary">
                Cerrar Detalle
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};
