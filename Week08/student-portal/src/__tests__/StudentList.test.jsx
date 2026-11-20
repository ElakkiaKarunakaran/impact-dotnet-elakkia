import { render, screen } from '@testing-library/react';
import StudentList from '../components/StudentList';

const STUDENTS = [
  { id: 1, name: 'Elakkia', age: 21, rollNumber: 'R001', email: 'e@test.com' },
  { id: 2, name: 'Ramya',   age: 22, rollNumber: 'R002', email: 'r@test.com' },
];

describe('StudentList', () => {
  test('renders a row for each student', () => {
    render(<StudentList students={STUDENTS} loading={false} canWrite={false} />);
    expect(screen.getByText('Elakkia')).toBeInTheDocument();
    expect(screen.getByText('Ramya')).toBeInTheDocument();
  });

  test('shows "No students found" when list is empty', () => {
    render(<StudentList students={[]} loading={false} canWrite={false} />);
    expect(screen.getByText(/no students found/i)).toBeInTheDocument();
  });

  test('shows loading indicator while loading', () => {
    render(<StudentList students={[]} loading={true} canWrite={false} />);
    expect(screen.getByLabelText('loading')).toBeInTheDocument();
  });

  test('renders all column headers', () => {
    render(<StudentList students={STUDENTS} loading={false} canWrite={false} />);
    expect(screen.getByText('Name')).toBeInTheDocument();
    expect(screen.getByText('Roll No.')).toBeInTheDocument();
    expect(screen.getByText('Email')).toBeInTheDocument();
  });

  test('renders correct number of data rows', () => {
    render(<StudentList students={STUDENTS} loading={false} canWrite={false} />);
    // 2 student rows + 1 header row
    const rows = screen.getAllByRole('row');
    expect(rows).toHaveLength(3);
  });
});
