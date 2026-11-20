import { render, screen, waitFor, fireEvent } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import StudentsPage from '../pages/StudentsPage';

const mockLogout   = jest.fn();
const mockNavigate = jest.fn();

// Default to Teacher role; individual tests can override via mockUseAuth
let mockUser = { username: 'teacher', role: 'Teacher' };

jest.mock('../context/AuthContext', () => ({
  useAuth: () => ({ user: mockUser, logout: mockLogout }),
}));

jest.mock('react-router-dom', () => ({
  ...jest.requireActual('react-router-dom'),
  useNavigate: () => mockNavigate,
}));

const mockGet    = jest.fn();
const mockDelete = jest.fn();
const mockPost   = jest.fn();

jest.mock('../api/axiosInstance', () => ({
  __esModule: true,
  default: {
    get:    (...args) => mockGet(...args),
    delete: (...args) => mockDelete(...args),
    post:   (...args) => mockPost(...args),
  },
  setTokenGetter: jest.fn(),
}));

const STUDENTS = [
  { id: 1, name: 'Elakkia', age: 21, rollNumber: 'R001', email: 'e@test.com' },
  { id: 2, name: 'Ramya',   age: 22, rollNumber: 'R002', email: 'r@test.com' },
];

function renderPage() {
  return render(
    <MemoryRouter>
      <StudentsPage />
    </MemoryRouter>
  );
}

describe('StudentsPage (Task 8.4 / 8.8)', () => {
  beforeEach(() => {
    mockGet.mockResolvedValue({ data: STUDENTS });
    mockLogout.mockReset();
    mockNavigate.mockReset();
    mockUser = { username: 'teacher', role: 'Teacher' };
  });

  test('fetches and renders students on mount', async () => {
    renderPage();
    await waitFor(() => expect(screen.getByText('Elakkia')).toBeInTheDocument());
    expect(screen.getByText('Ramya')).toBeInTheDocument();
    expect(mockGet).toHaveBeenCalledWith('/api/students');
  });

  test('shows loading indicator while fetching', () => {
    // Never resolves so loading stays true
    mockGet.mockReturnValue(new Promise(() => {}));
    renderPage();
    expect(screen.getByLabelText('loading')).toBeInTheDocument();
  });

  test('shows error message when fetch fails', async () => {
    mockGet.mockRejectedValue(new Error('Network error'));
    renderPage();
    await waitFor(() =>
      expect(screen.getByText(/failed to load/i)).toBeInTheDocument()
    );
  });

  test('Teacher sees the Add Student button', async () => {
    renderPage();
    await waitFor(() => expect(screen.getByTestId('create-btn')).toBeInTheDocument());
  });

  test('Student does NOT see the Add Student button', async () => {
    mockUser = { username: 'elakkia', role: 'Student' };
    renderPage();
    await waitFor(() => expect(screen.getByText('Elakkia')).toBeInTheDocument());
    expect(screen.queryByTestId('create-btn')).not.toBeInTheDocument();
  });

  test('clicking Add Student toggles the add form (Teacher only)', async () => {
    renderPage();
    await waitFor(() => expect(screen.getByTestId('create-btn')).toBeInTheDocument());
    fireEvent.click(screen.getByTestId('create-btn'));
    expect(screen.getByPlaceholderText('Name')).toBeInTheDocument();
    // Click Cancel to hide the form
    fireEvent.click(screen.getByTestId('create-btn'));
    expect(screen.queryByPlaceholderText('Name')).not.toBeInTheDocument();
  });

  test('deletes a student when confirmed', async () => {
    window.confirm = jest.fn().mockReturnValue(true);
    mockDelete.mockResolvedValue({});
    renderPage();
    await waitFor(() => expect(screen.getAllByRole('button', { name: /delete/i })).toHaveLength(2));
    fireEvent.click(screen.getAllByRole('button', { name: /delete/i })[0]);
    await waitFor(() => expect(mockDelete).toHaveBeenCalledWith('/api/students/1'));
  });

  test('handleAdd posts new student and hides form', async () => {
    const newStudent = { id: 3, name: 'Arjun', age: 20, rollNumber: 'R003', email: 'a@test.com' };
    mockPost.mockResolvedValue({ data: newStudent });
    renderPage();
    await waitFor(() => expect(screen.getByTestId('create-btn')).toBeInTheDocument());
    fireEvent.click(screen.getByTestId('create-btn'));
    fireEvent.change(screen.getByPlaceholderText('Name'),     { target: { value: 'Arjun' } });
    fireEvent.change(screen.getByPlaceholderText('Age'),      { target: { value: '20' } });
    fireEvent.change(screen.getByPlaceholderText('Roll No.'), { target: { value: 'R003' } });
    fireEvent.click(screen.getByRole('button', { name: /save/i }));
    await waitFor(() => expect(mockPost).toHaveBeenCalledWith('/api/students', expect.objectContaining({ name: 'Arjun' })));
    // Form should be hidden after save
    expect(screen.queryByPlaceholderText('Name')).not.toBeInTheDocument();
  });

  test('handleLogout clears auth and navigates to /login', async () => {
    renderPage();
    await waitFor(() => expect(screen.getByRole('button', { name: /logout/i })).toBeInTheDocument());
    fireEvent.click(screen.getByRole('button', { name: /logout/i }));
    expect(mockLogout).toHaveBeenCalled();
    expect(mockNavigate).toHaveBeenCalledWith('/login');
  });
});
