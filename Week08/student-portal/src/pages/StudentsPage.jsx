import { useState, useEffect } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import axiosInstance from '../api/axiosInstance';
import { ACADEMICS_PREFIX } from '../api/apiPaths';
import StudentList from '../components/StudentList';
import SearchBox from '../components/SearchBox';

// Task 8.4 — fetches students on mount
// Task 8.3 — search / filter
// Task 8.8 — role-based rendering: Teacher sees write controls, Student is read-only
export default function StudentsPage() {
  const { user, logout } = useAuth();
  const navigate          = useNavigate();
  const canWrite          = user?.role === 'Teacher';

  const [students, setStudents] = useState([]);
  const [loading,  setLoading]  = useState(true);
  const [search,   setSearch]   = useState('');
  const [error,    setError]    = useState('');

  // Add-student form state (Teacher only)
  const [showAdd, setShowAdd] = useState(false);
  const [newStudent, setNewStudent] = useState({ name: '', age: '', rollNumber: '', email: '' });

  useEffect(() => {
    fetchStudents();
  }, []);

  async function fetchStudents() {
    setLoading(true);
    try {
      const { data } = await axiosInstance.get(`${ACADEMICS_PREFIX}/api/students`);
      setStudents(data);
    } catch {
      setError('Failed to load students.');
    } finally {
      setLoading(false);
    }
  }

  async function handleDelete(id) {
    if (!window.confirm('Delete this student?')) return;
    try {
      await axiosInstance.delete(`${ACADEMICS_PREFIX}/api/students/${id}`);
      setStudents(prev => prev.filter(s => s.id !== id));
    } catch (err) {
      if (err.response?.status === 403)
        setError('Access denied — only Teachers can delete students.');
      else
        setError('Failed to delete student. Please try again.');
    }
  }

  async function handleAdd(e) {
    e.preventDefault();
    try {
      const { data } = await axiosInstance.post(`${ACADEMICS_PREFIX}/api/students`, {
        name: newStudent.name, age: parseInt(newStudent.age),
        rollNumber: newStudent.rollNumber, email: newStudent.email,
      });
      setStudents(prev => [...prev, data]);
      setNewStudent({ name: '', age: '', rollNumber: '', email: '' });
      setShowAdd(false);
    } catch (err) {
      if (err.response?.status === 403)
        setError('Access denied — only Teachers can add students.');
      else if (err.response?.status === 400)
        setError(err.response.data ?? 'Invalid student data.');
      else
        setError('Server error — please try again later.');
    }
  }

  function handleLogout() {
    logout();
    navigate('/login');
  }

  const filtered = students.filter(s =>
    s.name?.toLowerCase().includes(search.toLowerCase())
  );

  return (
    <>
      <nav>
        <span style={{ color: '#fff', fontWeight: 700, fontSize: '1.1rem' }}>Student Portal</span>
        <Link to="/reporting" className="btn btn-teal" style={{ marginLeft: '1rem' }}>Reporting</Link>
        <button className="btn btn-lime" onClick={handleLogout} style={{ marginLeft: 'auto' }}>
          Logout
        </button>
        <span className="role-badge">{user?.role}</span>
      </nav>

      <div className="page">
        <h1>Students</h1>

        {error && <p className="error">{error}</p>}

        <SearchBox value={search} onChange={setSearch} matchCount={filtered.length} />

        {/* Task 8.8 — Create button only for Teacher */}
        {canWrite && (
          <button
            className="btn btn-teal"
            onClick={() => setShowAdd(v => !v)}
            style={{ marginBottom: '1rem' }}
            data-testid="create-btn"
          >
            {showAdd ? 'Cancel' : '+ Add Student'}
          </button>
        )}

        {showAdd && canWrite && (
          <form onSubmit={handleAdd} style={{ background: '#fff', padding: '1rem', marginBottom: '1rem', borderRadius: 6 }}>
            <input placeholder="Name"       value={newStudent.name}       onChange={e => setNewStudent(p => ({ ...p, name: e.target.value }))}       required />
            <input placeholder="Age"        value={newStudent.age}        onChange={e => setNewStudent(p => ({ ...p, age: e.target.value }))}        type="number" required />
            <input placeholder="Roll No."   value={newStudent.rollNumber} onChange={e => setNewStudent(p => ({ ...p, rollNumber: e.target.value }))} required />
            <input placeholder="Email"      value={newStudent.email}      onChange={e => setNewStudent(p => ({ ...p, email: e.target.value }))}      type="email" />
            <button type="submit" className="btn btn-teal">Save</button>
          </form>
        )}

        <StudentList
          students={filtered}
          loading={loading}
          canWrite={canWrite}
          onEdit={() => {}}
          onDelete={handleDelete}
        />
      </div>
    </>
  );
}
