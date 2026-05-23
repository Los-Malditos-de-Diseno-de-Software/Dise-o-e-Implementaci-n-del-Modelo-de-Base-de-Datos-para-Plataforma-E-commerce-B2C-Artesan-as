import { describe, it, expect, vi, beforeEach } from 'vitest';
import { renderHook, waitFor } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import React from 'react';
import { useCart } from '../../hooks/useCart';
import { carritoApi } from '../../api/carrito.api';
import type { CartDto } from '../../types';

// Mock carritoApi
vi.mock('../../api/carrito.api', () => ({
  carritoApi: {
    getCart: vi.fn(),
    addCartItem: vi.fn(),
    removeCartItem: vi.fn(),
  },
}));

const mockCart: CartDto = {
  id: 'cart-123',
  sessionId: 'session-123',
  items: [
    {
      id: 'item-1',
      productoId: 'prod-1',
      productoNombre: 'Toro Celese',
      productoImagenBase64: 'fake',
      cantidad: 2,
      precioUnitarioCongelado: 50.00,
      subtotal: 100.00,
    },
  ],
  total: 100.00,
  totalItems: 2,
};

describe('useCart Hook', () => {
  let queryClient: QueryClient;
  let wrapper: React.FC<{ children: React.ReactNode }>;

  beforeEach(() => {
    vi.clearAllMocks();
    queryClient = new QueryClient({
      defaultOptions: {
        queries: {
          retry: false,
        },
      },
    });
    wrapper = ({ children }) => (
      <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>
    );
  });

  it('successfully fetches and returns cart data', async () => {
    vi.mocked(carritoApi.getCart).mockResolvedValue(mockCart);

    const { result } = renderHook(() => useCart(), { wrapper });

    // Initially loading
    expect(result.current.isLoading).toBe(true);

    await waitFor(() => expect(result.current.isLoading).toBe(false));

    expect(result.current.cart).toEqual(mockCart);
    expect(result.current.cart?.total).toBe(100.00);
    expect(result.current.cart?.totalItems).toBe(2);
  });

  it('calls addCartItem mutation and updates query cache', async () => {
    vi.mocked(carritoApi.getCart).mockResolvedValue({ id: 'cart-123', sessionId: 'session-123', items: [], total: 0, totalItems: 0 });
    const updatedCart = { ...mockCart };
    vi.mocked(carritoApi.addCartItem).mockResolvedValue(updatedCart);

    const { result } = renderHook(() => useCart(), { wrapper });

    await waitFor(() => expect(result.current.isLoading).toBe(false));

    // Call addItem
    await result.current.addItem({ productoId: 'prod-1', cantidad: 2 });

    expect(carritoApi.addCartItem).toHaveBeenCalledWith({ productoId: 'prod-1', cantidad: 2 });
    await waitFor(() => expect(result.current.cart).toEqual(updatedCart));
  });

  it('calls removeCartItem mutation and updates query cache', async () => {
    vi.mocked(carritoApi.getCart).mockResolvedValue(mockCart);
    const emptyCart = { id: 'cart-123', sessionId: 'session-123', items: [], total: 0, totalItems: 0 };
    vi.mocked(carritoApi.removeCartItem).mockResolvedValue(emptyCart);

    const { result } = renderHook(() => useCart(), { wrapper });

    await waitFor(() => expect(result.current.isLoading).toBe(false));

    // Call removeItem
    await result.current.removeItem('item-1');

    expect(carritoApi.removeCartItem).toHaveBeenCalledWith('item-1');
    await waitFor(() => expect(result.current.cart).toEqual(emptyCart));
  });
});
