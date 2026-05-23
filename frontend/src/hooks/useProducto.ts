import { useQuery } from '@tanstack/react-query';
import { productosApi } from '../api/productos.api';

export const useProducto = (id: string) => {
  return useQuery({
    queryKey: ['producto', id],
    queryFn: () => productosApi.getProductoById(id),
    enabled: !!id,
  });
};
