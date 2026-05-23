import { axiosClient } from './axiosClient';
import type { Result, LoginRequestDto, RegisterRequestDto, AuthResponseDto } from '../types';

export const authApi = {
  async login(params: LoginRequestDto) {
    const { data } = await axiosClient.post<Result<AuthResponseDto>>('/auth/login', params);
    return data.data;
  },

  async register(params: RegisterRequestDto) {
    const { data } = await axiosClient.post<Result<AuthResponseDto>>('/auth/register', params);
    return data.data;
  },
};
