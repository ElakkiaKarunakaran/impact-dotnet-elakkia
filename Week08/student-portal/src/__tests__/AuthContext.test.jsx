import { render, screen, act } from '@testing-library/react';
import { AuthProvider, useAuth } from '../context/AuthContext';

const mockPost = jest.fn();

jest.mock('../api/axiosInstance', () => ({
  __esModule: true,
  default: { post: (...args) => mockPost(...args) },
  setTokenGetter: jest.fn(),
}));

function TestComponent() {
  const { user, login, logout, register } = useAuth();
  return (
    <div>
      <span data-testid="username">{user?.username ?? 'none'}</span>
      <span data-testid="role">{user?.role ?? 'none'}</span>
      <button onClick={() => login('alice', 'pass123')}>Login</button>
      <button onClick={logout}>Logout</button>
      <button onClick={() => register('bob', 'pass123', 'Student')}>Register</button>
    </div>
  );
}

function Wrapper({ children }) {
  return <AuthProvider>{children}</AuthProvider>;
}

describe('AuthContext (Task 8.7)', () => {
  beforeEach(() => { mockPost.mockReset(); });

  test('initial user is null', () => {
    render(<TestComponent />, { wrapper: Wrapper });
    expect(screen.getByTestId('username')).toHaveTextContent('none');
    expect(screen.getByTestId('role')).toHaveTextContent('none');
  });

  test('login sets user and role from API response', async () => {
    mockPost.mockResolvedValue({ data: { token: 'jwt123', username: 'alice', role: 'Teacher' } });
    render(<TestComponent />, { wrapper: Wrapper });
    await act(async () => { screen.getByRole('button', { name: /^login$/i }).click(); });
    expect(screen.getByTestId('username')).toHaveTextContent('alice');
    expect(screen.getByTestId('role')).toHaveTextContent('Teacher');
  });

  test('logout clears user and role', async () => {
    mockPost.mockResolvedValue({ data: { token: 'jwt123', username: 'alice', role: 'Teacher' } });
    render(<TestComponent />, { wrapper: Wrapper });
    await act(async () => { screen.getByRole('button', { name: /^login$/i }).click(); });
    await act(async () => { screen.getByRole('button', { name: /^logout$/i }).click(); });
    expect(screen.getByTestId('username')).toHaveTextContent('none');
  });

  test('register calls the register API endpoint', async () => {
    mockPost.mockResolvedValue({ data: {} });
    render(<TestComponent />, { wrapper: Wrapper });
    await act(async () => { screen.getByRole('button', { name: /^register$/i }).click(); });
    expect(mockPost).toHaveBeenCalledWith(
      '/api/auth/register',
      { username: 'bob', password: 'pass123', role: 'Student' }
    );
  });
});
