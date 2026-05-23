import { useMutation, useQueryClient } from '@tanstack/react-query';
import { pedidosApi, type CreateOrderParams } from '../api/pedidos.api';

export const useCreateOrder = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (params: CreateOrderParams) => pedidosApi.createOrder(params),
    onSuccess: (data) => {
      // Invalidate cart since it's going to be emptied after successful payment
      queryClient.invalidateQueries({ queryKey: ['cart'] });
      
      // Redirect user to Stripe Checkout URL
      if (data.stripeCheckoutUrl) {
        window.location.href = data.stripeCheckoutUrl;
      }
    },
  });
};
