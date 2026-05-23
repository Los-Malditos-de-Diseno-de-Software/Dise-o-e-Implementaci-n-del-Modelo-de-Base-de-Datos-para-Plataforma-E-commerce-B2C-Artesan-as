import { axiosClient } from './axiosClient';
import type { Result, ArtesanoDto } from '../types';

export const artesanosApi = {
  async getArtesanos() {
    const { data } = await axiosClient.get<Result<ArtesanoDto[]>>('/artesanos');
    return data.data;
  },

  async getArtesanoById(id: string) {
    const { data } = await axiosClient.get<Result<ArtesanoDto>>(`/artesanos/${id}`);
    return data.data;
  },

  async createArtesano(params: Omit<ArtesanoDto, 'id'>) {
    const { data } = await axiosClient.post<Result<ArtesanoDto>>('/artesanos', params);
    return data.data;
  },

  async updateArtesano(id: string, params: Omit<ArtesanoDto, 'id'>) {
    const { data } = await axiosClient.put<Result<ArtesanoDto>>(`/artesanos/${id}`, params);
    return data.data;
  },

  async deleteArtesano(id: string) {
    const { data } = await axiosClient.delete<Result<boolean>>(`/artesanos/${id}`);
    return data.data;
  },
};
