import axios from 'axios';

// Task 8.7 — axios instance with base URL; interceptor attaches JWT Bearer header
const instance = axios.create({
  baseURL: 'http://localhost:5259',
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

export default instance;
