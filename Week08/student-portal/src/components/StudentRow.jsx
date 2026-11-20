// Task 8.2 — renders a single student row; accepts a student prop
// Task 8.8 — onEdit/onDelete only rendered for Teacher role (passed as props)
export default function StudentRow({ student, onEdit, onDelete, canWrite }) {
  return (
    <tr>
      <td>{student.id}</td>
      <td>{student.name}</td>
      <td>{student.age}</td>
      <td>{student.rollNumber}</td>
      <td>{student.email}</td>
      {canWrite && (
        <td className="controls">
          <button className="btn btn-lime"  onClick={() => onEdit(student)}>Edit</button>
          <button className="btn btn-red"   onClick={() => onDelete(student.id)}>Delete</button>
        </td>
      )}
      {!canWrite && <td>—</td>}
    </tr>
  );
}
