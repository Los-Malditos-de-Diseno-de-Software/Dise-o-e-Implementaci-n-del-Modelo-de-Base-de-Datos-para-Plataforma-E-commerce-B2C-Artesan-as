import { useQuery } from '@tanstack/react-query';
import { pedidosApi } from '../api/pedidos.api';

export const useAdminOrders = () => {
  const ordersQuery = useQuery({
    queryKey: ['admin-orders'],
    queryFn: () => pedidosApi.getAdminOrders(),
  });

  return {
    orders: ordersQuery.data || [],
    isLoading: ordersQuery.isLoading,
    error: ordersQuery.error,
    refetch: ordersQuery.refetch,
  };
};
