import React, { useState, useEffect } from 'react';
import { Mail, Lock, User, Phone, LogIn, UserPlus } from 'lucide-react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';

export const Register = () => {
  const navigate = useNavigate();
  const { register, isRegistering, registerError, isAuthenticated, user } = useAuth();

  const [nombre, setNombre] = useState('');
  const [apellido, setApellido] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [telefono, setTelefono] = useState('');
  const [errorMsg, setErrorMsg] = useState('');

  useEffect(() => {
    if (isAuthenticated) {
      if (user?.rol === 'Administrador') {
        navigate('/admin');
      } else {
        navigate('/');
      }
    }
  }, [isAuthenticated, user, navigate]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!nombre || !apellido || !email || !password) {
      setErrorMsg('Por favor rellena los campos requeridos.');
      return;
    }
    setErrorMsg('');
    try {
      await register({
        nombre,
        apellido,
        email,
        password,
        telefono: telefono || undefined
      });
    } catch (err: any) {
      console.error(err);
      setErrorMsg(err?.response?.data?.message || 'Error al registrar la cuenta. El correo podría estar en uso.');
    }
  };

  return (
    <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '80vh', padding: '2rem 0' }}>
      <div className="glass-panel" style={{
        width: '100%',
        maxWidth: '460px',
        padding: '2.5rem',
        border: '1px solid rgba(255,255,255,0.08)',
        boxShadow: 'var(--shadow-lg)'
      }}>
        {/* Header */}
        <div style={{ textAlign: 'center', marginBottom: '2rem' }}>
          <h2 className="text-gradient" style={{ fontSize: '2rem', marginBottom: '0.5rem' }}>Registro</h2>
          <p style={{ color: 'var(--text-secondary)', fontSize: '0.9rem' }}>
            Únete y descubre piezas de arte cusqueño únicas
          </p>
        </div>

        {/* Errors */}
        {(errorMsg || registerError) && (
          <div style={{
            backgroundColor: 'rgba(239, 68, 68, 0.1)',
            border: '1px solid var(--danger-color)',
            color: 'white',
            padding: '0.75rem 1rem',
            borderRadius: 'var(--radius-md)',
            marginBottom: '1.5rem',
            fontSize: '0.85rem'
          }}>
            {errorMsg || 'Ocurrió un error al crear la cuenta.'}
          </div>
        )}

        {/* Form */}
        <form onSubmit={handleSubmit}>
          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
            <div className="form-group">
              <label className="form-label" style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
                <User size={14} />
                <span>Nombre *</span>
              </label>
              <input
                type="text"
                required
                placeholder="Juan"
                value={nombre}
                onChange={(e) => setNombre(e.target.value)}
                className="form-input"
                disabled={isRegistering}
              />
            </div>
            <div className="form-group">
              <label className="form-label" style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
                <User size={14} />
                <span>Apellido *</span>
              </label>
              <input
                type="text"
                required
                placeholder="Perez"
                value={apellido}
                onChange={(e) => setApellido(e.target.value)}
                className="form-input"
                disabled={isRegistering}
              />
            </div>
          </div>

          <div className="form-group">
            <label className="form-label" style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
              <Mail size={16} />
              <span>Correo Electrónico *</span>
            </label>
            <input
              type="email"
              required
              placeholder="juan.perez@ejemplo.com"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              className="form-input"
              disabled={isRegistering}
            />
          </div>

          <div className="form-group">
            <label className="form-label" style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
              <Phone size={16} />
              <span>Teléfono</span>
            </label>
            <input
              type="tel"
              placeholder="+51 987 654 321"
              value={telefono}
              onChange={(e) => setTelefono(e.target.value)}
              className="form-input"
              disabled={isRegistering}
            />
          </div>

          <div className="form-group">
            <label className="form-label" style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
              <Lock size={16} />
              <span>Contraseña *</span>
            </label>
            <input
              type="password"
              required
              placeholder="Mínimo 6 caracteres"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              className="form-input"
              disabled={isRegistering}
            />
          </div>

          <button
            type="submit"
            className="btn btn-primary"
            style={{
              width: '100%',
              padding: '0.85rem',
              fontSize: '1rem',
              justifyContent: 'center',
              marginTop: '1rem'
            }}
            disabled={isRegistering}
          >
            {isRegistering ? (
              <span className="spinner-mini" />
            ) : (
              <>
                <UserPlus size={18} />
                <span>Registrar Cuenta</span>
              </>
            )}
          </button>
        </form>

        {/* Footer Link */}
        <div style={{ marginTop: '2rem', textAlign: 'center', fontSize: '0.875rem', color: 'var(--text-secondary)' }}>
          <span>¿Ya tienes una cuenta? </span>
          <Link to="/login" style={{ color: 'var(--accent-color)', fontWeight: '600', display: 'inline-flex', alignItems: 'center', gap: '4px' }}>
            <LogIn size={14} />
            <span>Inicia sesión aquí</span>
          </Link>
        </div>
      </div>
      <style>{`
        .spinner-mini {
          width: 16px;
          height: 16px;
          border: 2px solid rgba(255,255,255,0.2);
          border-top: 2px solid white;
          border-radius: 50%;
          animation: spin 0.6s linear infinite;
        }
      `}</style>
    </div>
  );
};
