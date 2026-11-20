import { render, screen } from '@testing-library/react';
import StudentList from '../components/StudentList';
import StudentRow  from '../components/StudentRow';

const STUDENTS = [
  { id: 1, name: 'Elakkia', age: 21, rollNumber: 'R001', email: 'e@test.com' },
  { id: 2, name: 'Ramya',   age: 22, rollNumber: 'R002', email: 'r@test.com' },
];

// Task 8.8 — role-based rendering: Teacher sees write controls; Student is read-only

describe('Teacher role', () => {
  test('Edit and Delete buttons are visible for each student', () => {
    render(
      <StudentList students={STUDENTS} loading={false} canWrite={true}
        onEdit={jest.fn()} onDelete={jest.fn()} />
    );
    expect(screen.getAllByRole('button', { name: /edit/i })).toHaveLength(2);
    expect(screen.getAllByRole('button', { name: /delete/i })).toHaveLength(2);
  });
});

describe('Student role', () => {
  test('no Edit or Delete buttons are rendered', () => {
    render(
      <StudentList students={STUDENTS} loading={false} canWrite={false}
        onEdit={jest.fn()} onDelete={jest.fn()} />
    );
    expect(screen.queryByRole('button', { name: /edit/i })).not.toBeInTheDocument();
    expect(screen.queryByRole('button', { name: /delete/i })).not.toBeInTheDocument();
  });
});

describe('StudentRow', () => {
  test('renders student data correctly', () => {
    render(
      <table><tbody>
        <StudentRow student={STUDENTS[0]} canWrite={false} onEdit={jest.fn()} onDelete={jest.fn()} />
      </tbody></table>
    );
    expect(screen.getByText('Elakkia')).toBeInTheDocument();
    expect(screen.getByText('R001')).toBeInTheDocument();
    expect(screen.getByText('e@test.com')).toBeInTheDocument();
  });

  test('calls onDelete when Delete is clicked (Teacher)', () => {
    const onDelete = jest.fn();
    render(
      <table><tbody>
        <StudentRow student={STUDENTS[0]} canWrite={true} onEdit={jest.fn()} onDelete={onDelete} />
      </tbody></table>
    );
    screen.getByRole('button', { name: /delete/i }).click();
    expect(onDelete).toHaveBeenCalledWith(1);
  });

  test('calls onEdit when Edit is clicked (Teacher)', () => {
    const onEdit = jest.fn();
    render(
      <table><tbody>
        <StudentRow student={STUDENTS[0]} canWrite={true} onEdit={onEdit} onDelete={jest.fn()} />
      </tbody></table>
    );
    screen.getByRole('button', { name: /edit/i }).click();
    expect(onEdit).toHaveBeenCalledWith(STUDENTS[0]);
  });
});
