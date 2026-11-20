import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import LoginPage from '../pages/LoginPage';

const mockLogin    = jest.fn();
const mockNavigate = jest.fn();

jest.mock('../context/AuthContext', () => ({
  useAuth: () => ({ login: mockLogin }),
}));

jest.mock('react-router-dom', () => ({
  ...jest.requireActual('react-router-dom'),
  useNavigate: () => mockNavigate,
}));

function renderPage() {
  return render(
    <MemoryRouter>
      <LoginPage />
    </MemoryRouter>
  );
}

describe('LoginPage (Task 8.6)', () => {
  beforeEach(() => {
    mockLogin.mockReset();
    mockNavigate.mockReset();
  });

  test('renders username and password fields', () => {
    renderPage();
    expect(screen.getByLabelText(/username/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/password/i)).toBeInTheDocument();
  });

  test('submit button is disabled when form is empty', () => {
    renderPage();
    expect(screen.getByRole('button', { name: /sign in/i })).toBeDisabled();
  });

  test('submit button is disabled when password is fewer than 6 chars', () => {
    renderPage();
    fireEvent.change(screen.getByLabelText(/username/i), { target: { value: 'alice' } });
    fireEvent.change(screen.getByLabelText(/password/i), { target: { value: '123' } });
    expect(screen.getByRole('button', { name: /sign in/i })).toBeDisabled();
  });

  test('submit button is enabled with valid credentials', () => {
    renderPage();
    fireEvent.change(screen.getByLabelText(/username/i), { target: { value: 'alice' } });
    fireEvent.change(screen.getByLabelText(/password/i), { target: { value: 'pass12' } });
    expect(screen.getByRole('button', { name: /sign in/i })).toBeEnabled();
  });

  test('navigates to /students on successful login', async () => {
    mockLogin.mockResolvedValue({});
    renderPage();
    fireEvent.change(screen.getByLabelText(/username/i), { target: { value: 'alice' } });
    fireEvent.change(screen.getByLabelText(/password/i), { target: { value: 'pass12' } });
    fireEvent.click(screen.getByRole('button', { name: /sign in/i }));
    await waitFor(() => expect(mockNavigate).toHaveBeenCalledWith('/students'));
  });

  test('shows error message on failed login', async () => {
    mockLogin.mockRejectedValue({ response: { data: 'Invalid credentials' } });
    renderPage();
    fireEvent.change(screen.getByLabelText(/username/i), { target: { value: 'alice' } });
    fireEvent.change(screen.getByLabelText(/password/i), { target: { value: 'pass12' } });
    fireEvent.click(screen.getByRole('button', { name: /sign in/i }));
    await waitFor(() =>
      expect(screen.getByRole('alert')).toHaveTextContent('Invalid credentials')
    );
  });
});
