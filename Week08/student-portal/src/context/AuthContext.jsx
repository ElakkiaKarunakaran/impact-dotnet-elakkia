import { createContext, useContext, useState } from 'react';
import axiosInstance, { setTokenGetter } from '../api/axiosInstance';
import { IDENTITY_PREFIX } from '../api/apiPaths';

const AuthContext = createContext(null);

// Task 8.7 — JWT stored in React context (memory, not localStorage)
export function AuthProvider({ children }) {
  const [token, setToken] = useState(null);
  const [user,  setUser]  = useState(null);

  // Keep the axios interceptor in sync with context token
  setTokenGetter(() => token);

  async function login(username, password) {
    const { data } = await axiosInstance.post(`${IDENTITY_PREFIX}/api/auth/login`, { username, password });
    setToken(data.token);
    setUser({ username: data.username, role: data.role });
    return data;
  }

  async function register(username, password, role) {
    const { data } = await axiosInstance.post(`${IDENTITY_PREFIX}/api/auth/register`, { username, password, role });
    return data;
  }

  function logout() {
    setToken(null);
    setUser(null);
  }

  return (
    <AuthContext.Provider value={{ token, user, login, logout, register }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  return useContext(AuthContext);
}
