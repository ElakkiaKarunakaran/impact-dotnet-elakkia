import { render, screen, waitFor, fireEvent } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import ReportingPage from '../pages/ReportingPage';

const mockLogout   = jest.fn();
const mockNavigate = jest.fn();

jest.mock('../context/AuthContext', () => ({
  useAuth: () => ({ user: { username: 'teacher', role: 'Teacher' }, logout: mockLogout }),
}));

jest.mock('react-router-dom', () => ({
  ...jest.requireActual('react-router-dom'),
  useNavigate: () => mockNavigate,
}));

const mockGet = jest.fn();

jest.mock('../api/axiosInstance', () => ({
  __esModule: true,
  default: { get: (...args) => mockGet(...args) },
  setTokenGetter: jest.fn(),
}));

const SUMMARY = [
  { courseCode: 'CS101', studentCount: 2 },
  { courseCode: 'CS201', studentCount: 2 },
];

function renderPage() {
  return render(
    <MemoryRouter>
      <ReportingPage />
    </MemoryRouter>
  );
}

describe('ReportingPage (Task 10.8)', () => {
  beforeEach(() => {
    mockGet.mockReset();
    mockLogout.mockReset();
    mockNavigate.mockReset();
  });

  test('fetches and renders the enrollment summary on mount', async () => {
    mockGet.mockResolvedValue({ data: SUMMARY });
    renderPage();
    await waitFor(() => expect(screen.getByText('CS101')).toBeInTheDocument());
    expect(screen.getByText('CS201')).toBeInTheDocument();
    expect(mockGet).toHaveBeenCalledWith('/api/enrollments/summary');
  });

  test('shows loading indicator while fetching', () => {
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

  test('handleLogout clears auth and navigates to /login', async () => {
    mockGet.mockResolvedValue({ data: SUMMARY });
    renderPage();
    await waitFor(() => expect(screen.getByRole('button', { name: /logout/i })).toBeInTheDocument());
    fireEvent.click(screen.getByRole('button', { name: /logout/i }));
    expect(mockLogout).toHaveBeenCalled();
    expect(mockNavigate).toHaveBeenCalledWith('/login');
  });
});
