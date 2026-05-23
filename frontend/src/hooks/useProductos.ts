import { useQuery } from '@tanstack/react-query';
import { productosApi, type GetProductosParams } from '../api/productos.api';

export const useProductos = (params?: GetProductosParams) => {
  return useQuery({
    queryKey: ['productos', params],
    queryFn: () => productosApi.getProductos(params),
  });
};
