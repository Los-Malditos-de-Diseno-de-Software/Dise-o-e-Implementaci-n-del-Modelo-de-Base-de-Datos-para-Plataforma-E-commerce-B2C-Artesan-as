import { useMutation, useQueryClient } from '@tanstack/react-query';
import { productosApi } from '../api/productos.api';

export const useAdminProductos = () => {
  const queryClient = useQueryClient();

  const createMutation = useMutation({
    mutationFn: (formData: FormData) => productosApi.createProducto(formData),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['productos'] });
    },
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, formData }: { id: string; formData: FormData }) =>
      productosApi.updateProducto(id, formData),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['productos'] });
      queryClient.invalidateQueries({ queryKey: ['producto', variables.id] });
    },
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) => productosApi.deleteProducto(id),
    onSuccess: (_, id) => {
      queryClient.invalidateQueries({ queryKey: ['productos'] });
      queryClient.invalidateQueries({ queryKey: ['producto', id] });
    },
  });

  return {
    createProducto: createMutation.mutateAsync,
    isCreating: createMutation.isPending,
    updateProducto: updateMutation.mutateAsync,
    isUpdating: updateMutation.isPending,
    deleteProducto: deleteMutation.mutateAsync,
    isDeleting: deleteMutation.isPending,
  };
};
