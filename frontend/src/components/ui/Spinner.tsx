import React from 'react';

interface SpinnerProps {
  size?: 'sm' | 'md' | 'lg';
  color?: string;
}

export const Spinner: React.FC<SpinnerProps> = ({ size = 'md', color = 'var(--accent-color)' }) => {
  const sizePx = size === 'sm' ? '24px' : size === 'md' ? '40px' : '64px';
  const borderPx = size === 'sm' ? '3px' : size === 'md' ? '4px' : '6px';

  return (
    <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', padding: '1rem' }}>
      <div
        style={{
          width: sizePx,
          height: sizePx,
          border: `${borderPx} solid rgba(255, 255, 255, 0.1)`,
          borderTop: `${borderPx} solid ${color}`,
          borderRadius: '50%',
          animation: 'spin 1s linear infinite',
          boxShadow: 'var(--shadow-glow)',
        }}
      />
      <style>{`
        @keyframes spin {
          0% { transform: rotate(0deg); }
          100% { transform: rotate(360deg); }
        }
      `}</style>
    </div>
  );
};
