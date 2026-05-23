import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { carritoApi, type AddCartItemParams } from '../api/carrito.api';

export const useCart = () => {
  const queryClient = useQueryClient();

  const cartQuery = useQuery({
    queryKey: ['cart'],
    queryFn: () => carritoApi.getCart(),
  });

  const addItemMutation = useMutation({
    mutationFn: (params: AddCartItemParams) => carritoApi.addCartItem(params),
    onSuccess: (data) => {
      // Invalidate cart query to refetch latest state, or set query data directly
      queryClient.setQueryData(['cart'], data);
    },
  });

  const removeItemMutation = useMutation({
    mutationFn: (itemId: string) => carritoApi.removeCartItem(itemId),
    onSuccess: (data) => {
      queryClient.setQueryData(['cart'], data);
    },
  });

  return {
    cart: cartQuery.data,
    isLoading: cartQuery.isLoading,
    isError: cartQuery.isError,
    error: cartQuery.error,
    addItem: addItemMutation.mutateAsync,
    isAdding: addItemMutation.isPending,
    removeItem: removeItemMutation.mutateAsync,
    isRemoving: removeItemMutation.isPending,
  };
};
