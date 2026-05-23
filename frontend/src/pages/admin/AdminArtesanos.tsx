import React, { useState } from 'react';
import { useAdminArtesanos } from '../../hooks/useAdminArtesanos';
import { Spinner } from '../../components/ui/Spinner';
import { Plus, Edit2, Trash2, X, Landmark } from 'lucide-react';
import styles from './AdminArtesanos.module.css';

export const AdminArtesanos = () => {
  const {
    artesanos,
    isLoading,
    createArtesano,
    isCreating,
    updateArtesano,
    isUpdating,
    deleteArtesano
  } = useAdminArtesanos();

  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);

  // Form State
  const [nombre, setNombre] = useState('');
  const [historiaBiografia, setHistoriaBiografia] = useState('');
  const [comunidadOrigen, setComunidadOrigen] = useState('');

  const openAddModal = () => {
    setEditingId(null);
    setNombre('');
    setHistoriaBiografia('');
    setComunidadOrigen('');
    setIsModalOpen(true);
  };

  const openEditModal = (artesano: any) => {
    setEditingId(artesano.id);
    setNombre(artesano.nombre);
    setHistoriaBiografia(artesano.historiaBiografia);
    setComunidadOrigen(artesano.comunidadOrigen);
    setIsModalOpen(true);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    const params = {
      nombre,
      historiaBiografia,
      comunidadOrigen
    };

    try {
      if (editingId) {
        await updateArtesano({ id: editingId, params });
      } else {
        await createArtesano(params);
      }
      setIsModalOpen(false);
    } catch (err) {
      console.error('Error submitting artisan', err);
    }
  };

  const handleDelete = async (id: string) => {
    if (window.confirm('¿Está seguro de que desea eliminar este maestro artesano? Su catálogo e historia serán archivados.')) {
      try {
        await deleteArtesano(id);
      } catch (err) {
        console.error('Error deleting artisan', err);
      }
    }
  };

  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <div>
          <h1 className="text-gradient">Maestros Artesanos</h1>
          <p style={{ color: 'var(--text-secondary)' }}>Administra las historias, biografías y comunidades originarias.</p>
        </div>
        <button onClick={openAddModal} className="btn btn-primary" style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
          <Plus size={18} />
          <span>Añadir Artesano</span>
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
                  <th>Nombre</th>
                  <th>Comunidad de Origen</th>
                  <th>Biografía / Trayectoria</th>
                  <th style={{ textAlign: 'right' }}>Acciones</th>
                </tr>
              </thead>
              <tbody>
                {artesanos.length === 0 ? (
                  <tr>
                    <td colSpan={4} style={{ textAlign: 'center', padding: '3rem 0', color: 'var(--text-secondary)' }}>
                      No hay artesanos registrados.
                    </td>
                  </tr>
                ) : (
                  artesanos.map(art => (
                    <tr key={art.id}>
                      <td className={styles.nameCell}>
                        <div style={{ display: 'flex', alignItems: 'center', gap: '0.75rem' }}>
                          <div className={styles.artAvatar}>
                            <Landmark size={18} />
                          </div>
                          <span className={styles.artName}>{art.nombre}</span>
                        </div>
                      </td>
                      <td>
                        <span className={styles.communityBadge}>{art.comunidadOrigen}</span>
                      </td>
                      <td className={styles.bioCell}>{art.historiaBiografia}</td>
                      <td style={{ textAlign: 'right' }}>
                        <div className={styles.actionsCell}>
                          <button onClick={() => openEditModal(art)} className={styles.editBtn} aria-label="Editar">
                            <Edit2 size={16} />
                          </button>
                          <button onClick={() => handleDelete(art.id)} className={styles.deleteBtn} aria-label="Eliminar">
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
              <h3>{editingId ? 'Editar Maestro Artesano' : 'Registrar Maestro Artesano'}</h3>
              <button onClick={() => setIsModalOpen(false)} className={styles.closeBtn}>
                <X size={20} />
              </button>
            </div>

            <form onSubmit={handleSubmit} className={styles.form}>
              <div className={styles.inputGroup}>
                <label>Nombre Completo</label>
                <input
                  type="text"
                  required
                  value={nombre}
                  onChange={e => setNombre(e.target.value)}
                  placeholder="Ej. Mamerto Sánchez"
                />
              </div>

              <div className={styles.inputGroup}>
                <label>Comunidad / Distrito de Origen</label>
                <input
                  type="text"
                  required
                  value={comunidadOrigen}
                  onChange={e => setComunidadOrigen(e.target.value)}
                  placeholder="Ej. Chinchero, Cusco"
                />
              </div>

              <div className={styles.inputGroup}>
                <label>Historia, Biografía y Trayectoria</label>
                <textarea
                  required
                  rows={5}
                  value={historiaBiografia}
                  onChange={e => setHistoriaBiografia(e.target.value)}
                  placeholder="Escribe la historia de vida del artesano, su legado y las técnicas tradicionales que utiliza..."
                />
              </div>

              <div className={styles.formActions}>
                <button type="button" onClick={() => setIsModalOpen(false)} className="btn btn-outline">
                  Cancelar
                </button>
                <button type="submit" disabled={isCreating || isUpdating} className="btn btn-primary">
                  {isCreating || isUpdating ? <Spinner size="sm" /> : (editingId ? 'Guardar Cambios' : 'Registrar')}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};
