import StudentRow from './StudentRow';

// Task 8.2 — maps a students array to StudentRow; Task 8.4 — loading + empty states
export default function StudentList({ students, loading, canWrite, onEdit, onDelete }) {
  if (loading) {
    return <p aria-label="loading">Loading students…</p>;
  }

  if (!students || students.length === 0) {
    return <p>No students found.</p>;
  }

  return (
    <table>
      <thead>
        <tr>
          <th>ID</th><th>Name</th><th>Age</th><th>Roll No.</th><th>Email</th>
          <th>Actions</th>
        </tr>
      </thead>
      <tbody>
        {students.map(s => (
          <StudentRow
            key={s.id}
            student={s}
            canWrite={canWrite}
            onEdit={onEdit}
            onDelete={onDelete}
          />
        ))}
      </tbody>
    </table>
  );
}
