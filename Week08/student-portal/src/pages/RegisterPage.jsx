import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

// Task 8.5 — registration form with controlled inputs and inline validation
// Fields: Name (→ Username), DOB (validation only), Designation (→ Role), Email, Password
export default function RegisterPage() {
  const { register } = useAuth();
  const navigate      = useNavigate();

  const [form,    setForm]    = useState({ name: '', dob: '', designation: '', email: '', password: '' });
  const [errors,  setErrors]  = useState({});
  const [success, setSuccess] = useState('');
  const [loading, setLoading] = useState(false);
  const [apiError,setApiError]= useState('');

  function validate(f) {
    const e = {};
    if (!f.name.trim())                                   e.name        = 'Name is required';
    if (!f.dob)                                           e.dob         = 'Date of birth is required';
    if (!f.designation.trim())                            e.designation = 'Designation is required';
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(f.email))     e.email       = 'Valid email is required';
    if (f.password.length < 6)                            e.password    = 'Password must be at least 6 characters';
    return e;
  }

  function handleChange(e) {
    const updated = { ...form, [e.target.name]: e.target.value };
    setForm(updated);
    setErrors(validate(updated));
  }

  const isValid = Object.keys(validate(form)).length === 0;

  async function handleSubmit(e) {
    e.preventDefault();
    const errs = validate(form);
    if (Object.keys(errs).length) { setErrors(errs); return; }

    setLoading(true);
    setApiError('');
    try {
      const role = form.designation.toLowerCase() === 'teacher' ? 'Teacher' : 'Student';
      await register(form.name.trim(), form.password, role);
      setSuccess('Registration successful! Redirecting to login…');
      setTimeout(() => navigate('/login'), 1500);
    } catch (err) {
      setApiError(err.response?.data ?? 'Registration failed.');
    } finally {
      setLoading(false);
    }
  }

  const Field = ({ id, label, type = 'text' }) => (
    <>
      <label htmlFor={id}>{label}</label>
      <input
        id={id}
        name={id}
        type={type}
        value={form[id]}
        onChange={handleChange}
        aria-describedby={errors[id] ? `${id}-error` : undefined}
      />
      {errors[id] && <p id={`${id}-error`} className="error">{errors[id]}</p>}
    </>
  );

  return (
    <div className="form-card">
      <h2>Register</h2>
      {success && <p className="success" role="status">{success}</p>}
      {apiError && <p className="error"  role="alert">{apiError}</p>}
      <form onSubmit={handleSubmit} noValidate>
        <Field id="name"        label="Full Name" />
        <Field id="dob"         label="Date of Birth" type="date" />
        <Field id="designation" label='Designation (type "Teacher" for teacher role)' />
        <Field id="email"       label="Email" type="email" />
        <Field id="password"    label="Password" type="password" />

        <button
          type="submit"
          className="btn btn-teal"
          disabled={!isValid || loading}
        >
          {loading ? 'Registering…' : 'Register'}
        </button>
      </form>
      <p style={{ marginTop: '1rem', fontSize: '0.9rem' }}>
        Already have an account? <Link to="/login">Sign in</Link>
      </p>
    </div>
  );
}
