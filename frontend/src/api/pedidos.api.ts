import { axiosClient } from './axiosClient';
import type { Result, OrderDto, CreateOrderResponseDto } from '../types';

export interface CreateOrderParams {
  usuarioId: string;
  direccionEnvio: string;
}

export const pedidosApi = {
  async getOrder(id: string) {
    const { data } = await axiosClient.get<Result<OrderDto>>(`/pedidos/${id}`);
    return data.data;
  },

  async createOrder(params: CreateOrderParams) {
    const { data } = await axiosClient.post<Result<CreateOrderResponseDto>>('/pedidos', params);
    return data.data;
  },

  async getAdminOrders() {
    const { data } = await axiosClient.get<Result<OrderDto[]>>('/pedidos');
    return data.data;
  },
};
