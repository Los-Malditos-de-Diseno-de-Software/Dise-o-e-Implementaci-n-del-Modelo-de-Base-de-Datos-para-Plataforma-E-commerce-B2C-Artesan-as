import React, { useState, useEffect } from 'react';
import { Mail, Lock, LogIn, UserPlus } from 'lucide-react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';

export const Login = () => {
  const navigate = useNavigate();
  const { login, isLoggingIn, loginError, isAuthenticated, user } = useAuth();

  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
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
    if (!email || !password) {
      setErrorMsg('Por favor rellena todos los campos.');
      return;
    }
    setErrorMsg('');
    try {
      await login({ email, password });
    } catch (err: any) {
      console.error(err);
      setErrorMsg(err?.response?.data?.message || 'Credenciales inválidas. Verifica tus datos.');
    }
  };

  return (
    <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '60vh', padding: '2rem 0' }}>
      <div className="glass-panel" style={{
        width: '100%',
        maxWidth: '420px',
        padding: '2.5rem',
        border: '1px solid rgba(255,255,255,0.08)',
        boxShadow: 'var(--shadow-lg)'
      }}>
        {/* Header */}
        <div style={{ textAlign: 'center', marginBottom: '2rem' }}>
          <h2 className="text-gradient" style={{ fontSize: '2rem', marginBottom: '0.5rem' }}>Ingresar</h2>
          <p style={{ color: 'var(--text-secondary)', fontSize: '0.9rem' }}>
            Accede a tu cuenta de artesano o cliente
          </p>
        </div>

        {/* Errors */}
        {(errorMsg || loginError) && (
          <div style={{
            backgroundColor: 'rgba(239, 68, 68, 0.1)',
            border: '1px solid var(--danger-color)',
            color: 'white',
            padding: '0.75rem 1rem',
            borderRadius: 'var(--radius-md)',
            marginBottom: '1.5rem',
            fontSize: '0.85rem'
          }}>
            {errorMsg || 'Ocurrió un error al iniciar sesión.'}
          </div>
        )}

        {/* Form */}
        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label className="form-label" style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
              <Mail size={16} />
              <span>Correo Electrónico</span>
            </label>
            <input
              type="email"
              required
              placeholder="correo@ejemplo.com"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              className="form-input"
              disabled={isLoggingIn}
            />
          </div>

          <div className="form-group">
            <label className="form-label" style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
              <Lock size={16} />
              <span>Contraseña</span>
            </label>
            <input
              type="password"
              required
              placeholder="••••••••"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              className="form-input"
              disabled={isLoggingIn}
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
            disabled={isLoggingIn}
          >
            {isLoggingIn ? (
              <span className="spinner-mini" />
            ) : (
              <>
                <LogIn size={18} />
                <span>Iniciar Sesión</span>
              </>
            )}
          </button>
        </form>

        {/* Footer Link */}
        <div style={{ marginTop: '2rem', textAlign: 'center', fontSize: '0.875rem', color: 'var(--text-secondary)' }}>
          <span>¿No tienes una cuenta? </span>
          <Link to="/registro" style={{ color: 'var(--accent-color)', fontWeight: '600', display: 'inline-flex', alignItems: 'center', gap: '4px' }}>
            <UserPlus size={14} />
            <span>Regístrate aquí</span>
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
