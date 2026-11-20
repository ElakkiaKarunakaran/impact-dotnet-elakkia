import { render, screen } from '@testing-library/react';
import { MemoryRouter, Routes, Route } from 'react-router-dom';
import ProtectedRoute from '../components/ProtectedRoute';

let mockToken = null;

jest.mock('../context/AuthContext', () => ({
  useAuth: () => ({ token: mockToken }),
}));

describe('ProtectedRoute (Task 8.6)', () => {
  test('renders children when a token is present', () => {
    mockToken = 'valid-jwt';
    render(
      <MemoryRouter initialEntries={['/students']}>
        <Routes>
          <Route path="/students" element={
            <ProtectedRoute><div>Protected Content</div></ProtectedRoute>
          } />
        </Routes>
      </MemoryRouter>
    );
    expect(screen.getByText('Protected Content')).toBeInTheDocument();
  });

  test('redirects to /login when no token', () => {
    mockToken = null;
    render(
      <MemoryRouter initialEntries={['/students']}>
        <Routes>
          <Route path="/login" element={<div>Login Page</div>} />
          <Route path="/students" element={
            <ProtectedRoute><div>Protected Content</div></ProtectedRoute>
          } />
        </Routes>
      </MemoryRouter>
    );
    expect(screen.queryByText('Protected Content')).not.toBeInTheDocument();
    expect(screen.getByText('Login Page')).toBeInTheDocument();
  });
});
