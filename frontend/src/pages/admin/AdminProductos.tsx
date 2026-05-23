import React, { useState } from 'react';
import { useProductos } from '../../hooks/useProductos';
import { useAdminProductos } from '../../hooks/useAdminProductos';
import { useAdminArtesanos } from '../../hooks/useAdminArtesanos';
import { Spinner } from '../../components/ui/Spinner';
import { Plus, Edit2, Trash2, X, Upload } from 'lucide-react';
import styles from './AdminProductos.module.css';

export const AdminProductos = () => {
  // Query hook to fetch all products
  const { data: productosResponse, isLoading: loadingProducts, refetch } = useProductos({ pageSize: 100 });
  const { artesanos, isLoading: loadingArtesanos } = useAdminArtesanos();
  const { createProducto, isCreating, updateProducto, isUpdating, deleteProducto } = useAdminProductos();

  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);

  // Form State
  const [nombre, setNombre] = useState('');
  const [descripcion, setDescripcion] = useState('');
  const [precio, setPrecio] = useState('');
  const [stock, setStock] = useState('');
  const [esUnico, setEsUnico] = useState(false);
  const [artesanoId, setArtesanoId] = useState('');
  const [file, setFile] = useState<File | null>(null);
  const [previewUrl, setPreviewUrl] = useState<string | null>(null);

  const openAddModal = () => {
    setEditingId(null);
    setNombre('');
    setDescripcion('');
    setPrecio('');
    setStock('');
    setEsUnico(false);
    setArtesanoId(artesanos[0]?.id || '');
    setFile(null);
    setPreviewUrl(null);
    setIsModalOpen(true);
  };

  const openEditModal = (producto: any) => {
    setEditingId(producto.id);
    setNombre(producto.nombre);
    setDescripcion(producto.descripcion);
    setPrecio(producto.precio.toString());
    setStock(producto.stock.toString());
    setEsUnico(producto.esUnico);
    setArtesanoId(producto.artesanoId);
    setFile(null);
    setPreviewUrl(producto.imagenBase64 ? `data:image/png;base64,${producto.imagenBase64}` : null);
    setIsModalOpen(true);
  };

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files[0]) {
      const selectedFile = e.target.files[0];
      setFile(selectedFile);
      setPreviewUrl(URL.createObjectURL(selectedFile));
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    const formData = new FormData();
    formData.append('Nombre', nombre);
    formData.append('Descripcion', descripcion);
    formData.append('Precio', parseFloat(precio).toString());
    formData.append('Stock', parseInt(stock).toString());
    formData.append('EsUnico', esUnico.toString());

    if (editingId) {
      if (file) {
        formData.append('NuevaImagen', file);
      }
      try {
        await updateProducto({ id: editingId, formData });
        setIsModalOpen(false);
        refetch();
      } catch (err) {
        console.error('Error updating product', err);
      }
    } else {
      formData.append('ArtesanoId', artesanoId);
      if (file) {
        formData.append('Imagen', file);
      }
      try {
        await createProducto(formData);
        setIsModalOpen(false);
        refetch();
      } catch (err) {
        console.error('Error creating product', err);
      }
    }
  };

  const handleDelete = async (id: string) => {
    if (window.confirm('¿Está seguro de que desea eliminar esta obra de arte?')) {
      try {
        await deleteProducto(id);
        refetch();
      } catch (err) {
        console.error('Error deleting product', err);
      }
    }
  };

  const isLoading = loadingProducts || loadingArtesanos;
  const products = productosResponse?.items || [];

  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <div>
          <h1 className="text-gradient">Gestión del Catálogo</h1>
          <p style={{ color: 'var(--text-secondary)' }}>Añade, edita o retira obras de arte andino.</p>
        </div>
        <button onClick={openAddModal} className="btn btn-primary" style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
          <Plus size={18} />
          <span>Añadir Obra</span>
        </button>
      </div>

      {isLoading ? (
        <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '40vh' }}>
          <Spinner size="lg" />
        </div>
      ) : (
        <div className={`glass-panel ${styles.tablePanel}`}>
          <div className={styles.tableResponsive}>
            <table className={styles.table}>
              <thead>
                <tr>
                  <th>Imagen</th>
                  <th>Nombre</th>
                  <th>Artesano</th>
                  <th>Precio</th>
                  <th>Stock</th>
                  <th>Modalidad</th>
                  <th style={{ textAlign: 'right' }}>Acciones</th>
                </tr>
              </thead>
              <tbody>
                {products.length === 0 ? (
                  <tr>
                    <td colSpan={7} style={{ textAlign: 'center', padding: '3rem 0', color: 'var(--text-secondary)' }}>
                      No hay obras registradas en el catálogo.
                    </td>
                  </tr>
                ) : (
                  products.map(prod => (
                    <tr key={prod.id}>
                      <td>
                        <div className={styles.imageWrapper}>
                          {prod.imagenBase64 ? (
                            <img src={prod.imagenBase64.startsWith('data:') ? prod.imagenBase64 : `data:image/png;base64,${prod.imagenBase64}`} alt={prod.nombre} className={styles.prodImg} />
                          ) : (
                            <div className={styles.noImg}>S/I</div>
                          )}
                        </div>
                      </td>
                      <td>
                        <div className={styles.nameCell}>
                          <span className={styles.prodName}>{prod.nombre}</span>
                          <span className={styles.prodDesc}>
                            {prod.descripcion ? (prod.descripcion.length > 60 ? `${prod.descripcion.substring(0, 60)}...` : prod.descripcion) : ''}
                          </span>
                        </div>
                      </td>
                      <td>{prod.artesanoNombre || 'Anonimo'}</td>
                      <td className={styles.priceCell}>S/. {prod.precio.toFixed(2)}</td>
                      <td>
                        <span className={`${styles.stockBadge} ${prod.stock > 0 ? styles.stockOk : styles.stockOut}`}>
                          {prod.stock} disp.
                        </span>
                      </td>
                      <td>
                        {prod.esUnico ? (
                          <span className={styles.badgeUnico}>Pieza Única</span>
                        ) : (
                          <span className={styles.badgeRegular}>Estándar</span>
                        )}
                      </td>
                      <td style={{ textAlign: 'right' }}>
                        <div className={styles.actionsCell}>
                          <button onClick={() => openEditModal(prod)} className={styles.editBtn} aria-label="Editar">
                            <Edit2 size={16} />
                          </button>
                          <button onClick={() => handleDelete(prod.id)} className={styles.deleteBtn} aria-label="Eliminar">
                            <Trash2 size={16} />
                          </button>
                        </div>
                      </td>
                    </tr>
                  )))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* Beautiful Modal Dialog */}
      {isModalOpen && (
        <div className={styles.modalOverlay}>
          <div className={`glass-panel ${styles.modalContent}`}>
            <div className={styles.modalHeader}>
              <h3>{editingId ? 'Editar Obra de Arte' : 'Registrar Nueva Obra'}</h3>
              <button onClick={() => setIsModalOpen(false)} className={styles.closeBtn}>
                <X size={20} />
              </button>
            </div>

            <form onSubmit={handleSubmit} className={styles.form}>
              <div className={styles.formGrid}>
                <div className={styles.inputGroup}>
                  <label>Nombre de la Obra</label>
                  <input
                    type="text"
                    required
                    value={nombre}
                    onChange={e => setNombre(e.target.value)}
                    placeholder="Ej. Vasija Andina Kusipata"
                  />
                </div>

                <div className={styles.inputGroup}>
                  <label>Artesano Creador</label>
                  <select
                    disabled={!!editingId}
                    value={artesanoId}
                    onChange={e => setArtesanoId(e.target.value)}
                  >
                    {artesanos.map(art => (
                      <option key={art.id} value={art.id}>{art.nombre}</option>
                    ))}
                  </select>
                </div>
              </div>

              <div className={styles.inputGroup}>
                <label>Descripción y Simbología</label>
                <textarea
                  required
                  rows={3}
                  value={descripcion}
                  onChange={e => setDescripcion(e.target.value)}
                  placeholder="Explica la historia, simbología, materiales y técnicas empleadas en esta pieza..."
                />
              </div>

              <div className={styles.formGrid3}>
                <div className={styles.inputGroup}>
                  <label>Precio (S/.)</label>
                  <input
                    type="number"
                    step="0.01"
                    required
                    value={precio}
                    onChange={e => setPrecio(e.target.value)}
                    placeholder="85.00"
                  />
                </div>

                <div className={styles.inputGroup}>
                  <label>Cantidad en Stock</label>
                  <input
                    type="number"
                    required
                    value={stock}
                    onChange={e => setStock(e.target.value)}
                    placeholder="5"
                  />
                </div>

                <div className={styles.checkboxGroup}>
                  <label className={styles.checkboxLabel}>
                    <input
                      type="checkbox"
                      checked={esUnico}
                      onChange={e => setEsUnico(e.target.checked)}
                    />
                    <span>¿Es Pieza Única?</span>
                  </label>
                </div>
              </div>

              <div className={styles.imageUploadSection}>
                <label>Fotografía de la Obra</label>
                <div className={styles.uploadBox}>
                  <input
                    type="file"
                    id="fileUpload"
                    accept="image/*"
                    onChange={handleFileChange}
                    className={styles.fileInput}
                  />
                  <label htmlFor="fileUpload" className={styles.uploadLabel}>
                    {previewUrl ? (
                      <div className={styles.previewContainer}>
                        <img src={previewUrl} alt="Preview" className={styles.previewImg} />
                        <span className={styles.changeLabel}>Reemplazar Imagen</span>
                      </div>
                    ) : (
                      <div className={styles.uploadPlaceholder}>
                        <Upload size={32} />
                        <span>Sube o arrastra la foto del producto</span>
                      </div>
                    )}
                  </label>
                </div>
              </div>

              <div className={styles.formActions}>
                <button type="button" onClick={() => setIsModalOpen(false)} className="btn btn-outline">
                  Cancelar
                </button>
                <button type="submit" disabled={isCreating || isUpdating} className="btn btn-primary">
                  {isCreating || isUpdating ? <Spinner size="sm" /> : (editingId ? 'Guardar Cambios' : 'Registrar Obra')}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};
