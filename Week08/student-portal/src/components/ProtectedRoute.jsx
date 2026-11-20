import { Navigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

// Task 8.6 — redirects unauthenticated users to /login
export default function ProtectedRoute({ children }) {
  const { token } = useAuth();
  if (!token) return <Navigate to="/login" replace />;
  return children;
}
