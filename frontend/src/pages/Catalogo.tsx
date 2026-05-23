import React, { useState } from 'react';
import { Search } from 'lucide-react';
import { useProductos } from '../hooks/useProductos';
import { ProductCard } from '../components/ui/ProductCard';
import { Spinner } from '../components/ui/Spinner';

export const Catalogo = () => {
  const [searchTerm, setSearchTerm] = useState('');
  
  // Enlazar TanStack Query con el término de búsqueda
  const { data, isLoading, isError, error } = useProductos({
    search: searchTerm,
    pageSize: 20
  });

  const handleSearchChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setSearchTerm(e.target.value);
  };

  const productos = data?.items || [];

  return (
    <div style={{ paddingBottom: '3rem' }}>
      {/* Title Header */}
      <div style={{ textAlign: 'center', margin: '2rem 0 3rem' }}>
        <h1 className="text-gradient" style={{ fontSize: '2.5rem', marginBottom: '0.5rem' }}>
          Colección Cusco
        </h1>
        <p style={{ color: 'var(--text-secondary)', fontSize: '1.1rem', maxWidth: '600px', margin: '0 auto' }}>
          Obras hechas a mano y curadas con amor por maestros artesanos locales.
        </p>
      </div>

      {/* Search and Filters Bar */}
      <div className="glass-panel" style={{
        padding: '1.25rem',
        marginBottom: '2.5rem',
        display: 'flex',
        alignItems: 'center',
        gap: '1rem',
        border: '1px solid rgba(255,255,255,0.08)'
      }}>
        <div style={{ position: 'relative', flex: 1 }}>
          <Search size={20} color="var(--text-secondary)" style={{
            position: 'absolute',
            left: '12px',
            top: '50%',
            transform: 'translateY(-50%)',
            pointerEvents: 'none'
          }} />
          <input
            type="text"
            placeholder="Buscar artesanía, categoría o artesano..."
            value={searchTerm}
            onChange={handleSearchChange}
            className="form-input"
            style={{
              paddingLeft: '2.5rem',
              backgroundColor: 'rgba(0,0,0,0.3)',
              border: '1px solid var(--border-color)',
            }}
          />
        </div>
      </div>

      {/* Main Grid or Loading States */}
      {isLoading ? (
        <div style={{ padding: '5rem 0' }}>
          <Spinner size="lg" />
          <p style={{ textAlign: 'center', color: 'var(--text-secondary)', marginTop: '1rem' }}>
            Buscando en los talleres artesanos...
          </p>
        </div>
      ) : isError ? (
        <div className="glass-panel" style={{ padding: '3rem', textAlign: 'center', border: '1px solid rgba(239, 68, 68, 0.2)' }}>
          <h3 style={{ color: 'var(--danger-color)' }}>Error de carga</h3>
          <p style={{ color: 'var(--text-secondary)', marginTop: '0.5rem' }}>
            {(error as any)?.message || 'No se pudo conectar con el servidor.'}
          </p>
          <button className="btn btn-outline" style={{ marginTop: '1.5rem' }} onClick={() => window.location.reload()}>
            Reintentar
          </button>
        </div>
      ) : productos.length === 0 ? (
        <div style={{ textAlign: 'center', padding: '5rem 0' }}>
          <h3 style={{ color: 'var(--text-secondary)', fontSize: '1.5rem' }}>No se encontraron piezas</h3>
          <p style={{ color: 'var(--text-secondary)', marginTop: '0.5rem' }}>
            Prueba buscando con palabras clave diferentes o explora otra categoría.
          </p>
        </div>
      ) : (
        <div className="grid-cards animate-fade-in">
          {productos.map((producto) => (
            <ProductCard key={producto.id} producto={producto} />
          ))}
        </div>
      )}
    </div>
  );
};
