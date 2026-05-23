import { axiosClient } from './axiosClient';
import type { Result, ProductoDto } from '../types';

export interface GetProductosParams {
  page?: number;
  pageSize?: number;
  search?: string;
}

export const productosApi = {
  async getProductos(params?: GetProductosParams) {
    const { data } = await axiosClient.get<Result<{ items: ProductoDto[]; total: number }>>('/productos', {
      params: {
        page: params?.page || 1,
        pageSize: params?.pageSize || 12,
        search: params?.search || '',
      },
    });
    return data.data;
  },

  async getProductoById(id: string) {
    const { data } = await axiosClient.get<Result<ProductoDto>>(`/productos/${id}`);
    return data.data;
  },

  async createProducto(formData: FormData) {
    const { data } = await axiosClient.post<Result<ProductoDto>>('/productos', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
    return data.data;
  },

  async updateProducto(id: string, formData: FormData) {
    const { data } = await axiosClient.put<Result<ProductoDto>>(`/productos/${id}`, formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
    return data.data;
  },

  async deleteProducto(id: string) {
    const { data } = await axiosClient.delete<Result<boolean>>(`/productos/${id}`);
    return data.data;
  },
};
