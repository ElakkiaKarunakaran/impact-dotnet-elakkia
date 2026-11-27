import { useEffect, useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import axiosInstance from '../api/axiosInstance';
import { REPORTING_PREFIX } from '../api/apiPaths';

// Task 10.8 — reporting view: enrollment summary served by the Reporting
// microservice (EF DB First) via the gateway's /reporting/* route.
export default function ReportingPage() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const [summary, setSummary] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    (async () => {
      try {
        const { data } = await axiosInstance.get(`${REPORTING_PREFIX}/api/enrollments/summary`);
        setSummary(data);
      } catch {
        setError('Failed to load the enrollment summary.');
      } finally {
        setLoading(false);
      }
    })();
  }, []);

  function handleLogout() {
    logout();
    navigate('/login');
  }

  return (
    <>
      <nav>
        <span style={{ color: '#fff', fontWeight: 700, fontSize: '1.1rem' }}>Student Portal</span>
        <Link to="/students" className="btn btn-teal" style={{ marginLeft: '1rem' }}>Students</Link>
        <button className="btn btn-lime" onClick={handleLogout} style={{ marginLeft: 'auto' }}>
          Logout
        </button>
        <span className="role-badge">{user?.role}</span>
      </nav>

      <div className="page">
        <h1>Enrollment Summary</h1>
        {error && <p className="error">{error}</p>}
        {loading ? (
          <p aria-label="loading">Loading…</p>
        ) : (
          <table>
            <thead>
              <tr><th>Course Code</th><th>Student Count</th></tr>
            </thead>
            <tbody>
              {summary.map(row => (
                <tr key={row.courseCode}>
                  <td>{row.courseCode}</td>
                  <td>{row.studentCount}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
    </>
  );
}
