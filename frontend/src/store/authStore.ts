import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import type { AuthResponseDto } from '../types';

interface AuthState {
  user: AuthResponseDto | null;
  token: string | null;
  isAuthenticated: boolean;
  login: (data: AuthResponseDto) => void;
  logout: () => void;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      user: null,
      token: null,
      isAuthenticated: false,
      login: (data: AuthResponseDto) => 
        set({ user: data, token: data.token, isAuthenticated: true }),
      logout: () => 
        set({ user: null, token: null, isAuthenticated: false }),
    }),
    {
      name: 'auth-storage', // name of item in the storage (must be unique)
    }
  )
);
