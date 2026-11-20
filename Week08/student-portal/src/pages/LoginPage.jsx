import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

// Task 8.6 — login form; on success stores JWT and navigates to /students
export default function LoginPage() {
  const { login } = useAuth();
  const navigate   = useNavigate();

  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error,    setError]    = useState('');
  const [loading,  setLoading]  = useState(false);

  const isValid = username.trim().length > 0 && password.length >= 6;

  async function handleSubmit(e) {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      await login(username.trim(), password);
      navigate('/students');
    } catch (err) {
      setError(err.response?.data ?? 'Login failed. Check your credentials.');
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="form-card">
      <h2>Student Portal — Login</h2>
      <form onSubmit={handleSubmit} noValidate>
        <label htmlFor="username">Username</label>
        <input
          id="username"
          type="text"
          value={username}
          onChange={e => setUsername(e.target.value)}
          autoComplete="username"
        />

        <label htmlFor="password">Password</label>
        <input
          id="password"
          type="password"
          value={password}
          onChange={e => setPassword(e.target.value)}
          autoComplete="current-password"
        />

        {error && <p className="error" role="alert">{error}</p>}

        <button
          type="submit"
          className="btn btn-teal"
          disabled={!isValid || loading}
        >
          {loading ? 'Signing in…' : 'Sign in'}
        </button>
      </form>
      <p style={{ marginTop: '1rem', fontSize: '0.9rem' }}>
        No account? <Link to="/register">Register</Link>
      </p>
      <p style={{ fontSize: '0.8rem', color: '#777', marginTop: '0.5rem' }}>
        Dev accounts: <code>admin / admin123!</code> (Teacher) ·{' '}
        <code>elakkia / elakkia123!</code> (Student)
      </p>
    </div>
  );
}
