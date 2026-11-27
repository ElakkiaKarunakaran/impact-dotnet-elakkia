import axios from 'axios';

// Task 8.7 — axios instance with base URL; interceptor attaches JWT Bearer header
// Task 10.5 — base URL is overridable via window.__ENV__ (injected at container
// start, see Week10/docker-compose.yml) so the SAME build can point at either
// the Week07 monolith directly, or the Week10 gateway (http://localhost:8080).
// A plain global (not import.meta.env) keeps this file parseable by Jest/Babel
// with zero extra tooling.
const instance = axios.create({
  baseURL: (typeof window !== 'undefined' && window.__ENV__?.API_BASE_URL) || 'http://localhost:5259',
});

let _getToken = () => null;

// Called by AuthContext to keep the interceptor in sync with the stored token
export function setTokenGetter(fn) {
  _getToken = fn;
}

// Every outgoing request gets Authorization: Bearer <token> if a token exists
instance.interceptors.request.use(config => {
  const token = _getToken();
  if (token) {
    config.headers = config.headers ?? {};
    config.headers['Authorization'] = `Bearer ${token}`;
  }
  return config;
});

// Task 9.2 — handle 401 (token expired/invalid) → redirect to login
instance.interceptors.response.use(
  response => response,
  error => {
    if (
      error.response?.status === 401 &&
      typeof window !== 'undefined' &&
      window.location.pathname !== '/login'
    ) {
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export default instance;
