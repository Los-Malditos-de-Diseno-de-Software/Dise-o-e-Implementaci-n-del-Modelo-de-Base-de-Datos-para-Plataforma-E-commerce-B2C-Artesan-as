import { axiosClient } from './axiosClient';
import type { Result, CartDto } from '../types';

export interface AddCartItemParams {
  productoId: string;
  cantidad: number;
}

export const carritoApi = {
  async getCart() {
    const { data } = await axiosClient.get<Result<CartDto>>('/carrito');
    return data.data;
  },

  async addCartItem(params: AddCartItemParams) {
    const { data } = await axiosClient.post<Result<CartDto>>('/carrito/items', params);
    return data.data;
  },

  async removeCartItem(itemId: string) {
    const { data } = await axiosClient.delete<Result<CartDto>>(`/carrito/items/${itemId}`);
    return data.data;
  },
};
