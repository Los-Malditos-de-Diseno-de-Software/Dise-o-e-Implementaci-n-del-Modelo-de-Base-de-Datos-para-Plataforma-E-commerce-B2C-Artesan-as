import axios from 'axios';
import { v4 as uuidv4 } from 'uuid';

const API_BASE_URL = 'http://localhost:5050/api/v1';

export const axiosClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Helper para manejar el X-Session-Id (carrito anónimo)
const getOrCreateSessionId = () => {
  let sessionId = localStorage.getItem('artesanias_session_id');
  if (!sessionId) {
    sessionId = uuidv4();
    localStorage.setItem('artesanias_session_id', sessionId);
  }
  return sessionId;
};

// Interceptor para inyectar token y X-Session-Id
axiosClient.interceptors.request.use((config) => {
  // Inyectar Session ID siempre
  config.headers['X-Session-Id'] = getOrCreateSessionId();

  // Inyectar Token de Auth si existe (se asume que Zustand guardará esto en localStorage)
  const authStorageStr = localStorage.getItem('auth-storage');
  if (authStorageStr) {
    try {
      const authData = JSON.parse(authStorageStr);
      const token = authData?.state?.token;
      if (token) {
        config.headers['Authorization'] = `Bearer ${token}`;
      }
    } catch (e) {
      console.error('Error parsing auth storage', e);
    }
  }

  return config;
});
