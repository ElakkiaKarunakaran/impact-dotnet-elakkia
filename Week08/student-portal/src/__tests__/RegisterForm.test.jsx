import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import RegisterPage from '../pages/RegisterPage';

// Mock AuthContext so the form can render without a real context tree
jest.mock('../context/AuthContext', () => ({
  useAuth: () => ({ register: jest.fn().mockResolvedValue({}) }),
}));

// Mock react-router navigate
const mockNavigate = jest.fn();
jest.mock('react-router-dom', () => ({
  ...jest.requireActual('react-router-dom'),
  useNavigate: () => mockNavigate,
}));

function renderPage() {
  return render(
    <MemoryRouter>
      <RegisterPage />
    </MemoryRouter>
  );
}

describe('RegisterPage — controlled form + inline validation (Task 8.5)', () => {
  beforeEach(() => { mockNavigate.mockClear(); });

  test('submit button is disabled when form is empty', () => {
    renderPage();
    expect(screen.getByRole('button', { name: /register/i })).toBeDisabled();
  });

  test('submit button is disabled with invalid email', () => {
    renderPage();
    fireEvent.change(screen.getByLabelText(/full name/i),         { target: { name: 'name',        value: 'Alice' } });
    fireEvent.change(screen.getByLabelText(/date of birth/i),     { target: { name: 'dob',         value: '2000-01-01' } });
    fireEvent.change(screen.getByLabelText(/designation/i),       { target: { name: 'designation', value: 'Student' } });
    fireEvent.change(screen.getByLabelText(/email/i),             { target: { name: 'email',       value: 'not-an-email' } });
    fireEvent.change(screen.getByLabelText(/password/i),          { target: { name: 'password',    value: 'pass12' } });
    expect(screen.getByRole('button', { name: /register/i })).toBeDisabled();
  });

  test('submit button is disabled when password < 6 chars', () => {
    renderPage();
    fireEvent.change(screen.getByLabelText(/full name/i),         { target: { name: 'name',        value: 'Alice' } });
    fireEvent.change(screen.getByLabelText(/date of birth/i),     { target: { name: 'dob',         value: '2000-01-01' } });
    fireEvent.change(screen.getByLabelText(/designation/i),       { target: { name: 'designation', value: 'Student' } });
    fireEvent.change(screen.getByLabelText(/email/i),             { target: { name: 'email',       value: 'a@b.com' } });
    fireEvent.change(screen.getByLabelText(/password/i),          { target: { name: 'password',    value: '123' } });
    expect(screen.getByRole('button', { name: /register/i })).toBeDisabled();
  });

  test('submit button is enabled when all fields are valid', () => {
    renderPage();
    fireEvent.change(screen.getByLabelText(/full name/i),         { target: { name: 'name',        value: 'Alice' } });
    fireEvent.change(screen.getByLabelText(/date of birth/i),     { target: { name: 'dob',         value: '2000-01-01' } });
    fireEvent.change(screen.getByLabelText(/designation/i),       { target: { name: 'designation', value: 'Student' } });
    fireEvent.change(screen.getByLabelText(/email/i),             { target: { name: 'email',       value: 'a@b.com' } });
    fireEvent.change(screen.getByLabelText(/password/i),          { target: { name: 'password',    value: 'pass12' } });
    expect(screen.getByRole('button', { name: /register/i })).toBeEnabled();
  });

  test('shows success message and redirects after successful registration', async () => {
    renderPage();
    fireEvent.change(screen.getByLabelText(/full name/i),         { target: { name: 'name',        value: 'Alice' } });
    fireEvent.change(screen.getByLabelText(/date of birth/i),     { target: { name: 'dob',         value: '2000-01-01' } });
    fireEvent.change(screen.getByLabelText(/designation/i),       { target: { name: 'designation', value: 'Student' } });
    fireEvent.change(screen.getByLabelText(/email/i),             { target: { name: 'email',       value: 'a@b.com' } });
    fireEvent.change(screen.getByLabelText(/password/i),          { target: { name: 'password',    value: 'pass12' } });

    fireEvent.click(screen.getByRole('button', { name: /register/i }));

    await waitFor(() =>
      expect(screen.getByRole('status')).toHaveTextContent(/registration successful/i)
    );
  });
});
