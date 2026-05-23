import { describe, it, expect, vi } from 'vitest';
import { render, screen, fireEvent } from '@testing-library/react';
import { ProductCard } from '../../components/ui/ProductCard';
import type { ProductoDto } from '../../types';

// Mock Lucide icons
vi.mock('lucide-react', () => ({
  ShoppingCart: () => <span data-testid="shopping-cart-icon" />,
}));

// Mock useCart hook
const mockAddItem = vi.fn();
vi.mock('../../hooks/useCart', () => ({
  useCart: () => ({
    addItem: mockAddItem,
    isAdding: false,
  }),
}));

const mockProduct: ProductoDto = {
  id: 'product-1',
  artesanoId: 'artesano-1',
  artesanoNombre: 'Pedro Choque',
  nombre: 'Toro de Pucará Celeste',
  descripcion: 'Hermosa pieza artesanal decorativa andina.',
  precio: 75.00,
  stock: 5,
  esUnico: false,
  imagenBase64: 'fakebase64string',
  isDeleted: false,
  createdAt: '2026-05-21T00:00:00Z',
};

describe('ProductCard Component', () => {
  it('renders product details correctly', () => {
    render(<ProductCard producto={mockProduct} />);

    expect(screen.getByText('Pedro Choque')).toBeInTheDocument();
    expect(screen.getByText('Toro de Pucará Celeste')).toBeInTheDocument();
    expect(screen.getByText('Hermosa pieza artesanal decorativa andina.')).toBeInTheDocument();
    expect(screen.getByText(/75[.,]00/)).toBeInTheDocument(); // robust currency matcher
    expect(screen.getByRole('button', { name: /Agregar/i })).toBeInTheDocument();
  });

  it('renders "Pieza Única" badge when product is unique', () => {
    const uniqueProduct = { ...mockProduct, esUnico: true };
    render(<ProductCard producto={uniqueProduct} />);

    expect(screen.getByText('Pieza Única')).toBeInTheDocument();
  });

  it('renders "Agotado" badge and disables button when stock is 0', () => {
    const outOfStockProduct = { ...mockProduct, stock: 0 };
    render(<ProductCard producto={outOfStockProduct} />);

    expect(screen.getAllByText('Agotado')[0]).toBeInTheDocument();
    const button = screen.getByRole('button', { name: /Agotado/i });
    expect(button).toBeDisabled();
  });

  it('calls addItem when "Agregar" button is clicked', async () => {
    render(<ProductCard producto={mockProduct} />);

    const button = screen.getByRole('button', { name: /Agregar/i });
    fireEvent.click(button);

    expect(mockAddItem).toHaveBeenCalledWith({
      productoId: 'product-1',
      cantidad: 1,
    });
  });
});
