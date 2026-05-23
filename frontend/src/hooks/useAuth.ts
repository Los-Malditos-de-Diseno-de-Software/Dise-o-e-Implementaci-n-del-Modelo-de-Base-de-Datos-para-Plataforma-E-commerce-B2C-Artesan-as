import { useMutation } from '@tanstack/react-query';
import { authApi } from '../api/auth.api';
import { useAuthStore } from '../store/authStore';
import type { LoginRequestDto, RegisterRequestDto } from '../types';

export const useAuth = () => {
  const loginStore = useAuthStore((state) => state.login);
  const logoutStore = useAuthStore((state) => state.logout);
  const user = useAuthStore((state) => state.user);
  const token = useAuthStore((state) => state.token);
  const isAuthenticated = useAuthStore((state) => state.isAuthenticated);

  const loginMutation = useMutation({
    mutationFn: (params: LoginRequestDto) => authApi.login(params),
    onSuccess: (data) => {
      loginStore(data);
    },
  });

  const registerMutation = useMutation({
    mutationFn: (params: RegisterRequestDto) => authApi.register(params),
    onSuccess: (data) => {
      loginStore(data);
    },
  });

  return {
    user,
    token,
    isAuthenticated,
    login: loginMutation.mutateAsync,
    isLoggingIn: loginMutation.isPending,
    loginError: loginMutation.error,
    register: registerMutation.mutateAsync,
    isRegistering: registerMutation.isPending,
    registerError: registerMutation.error,
    logout: logoutStore,
  };
};
