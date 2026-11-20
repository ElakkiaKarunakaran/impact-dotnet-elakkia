import axiosInstance, { setTokenGetter } from '../api/axiosInstance';

// Task 8.7 — the axios interceptor must attach Authorization: Bearer <token>
// on every outgoing request when a token is present.
// Task 9.2 — response interceptor redirects to /login on 401.

describe('axios request interceptor', () => {
  afterEach(() => {
    setTokenGetter(() => null);
  });

  function runRequest(config = { headers: {} }) {
    const handler = axiosInstance.interceptors.request.handlers[0];
    return handler.fulfilled(config);
  }

  test('attaches Authorization: Bearer header when token is present', () => {
    setTokenGetter(() => 'my-jwt-token');
    const result = runRequest({ headers: {} });
    expect(result.headers['Authorization']).toBe('Bearer my-jwt-token');
  });

  test('does NOT add Authorization header when no token', () => {
    setTokenGetter(() => null);
    const result = runRequest({ headers: {} });
    expect(result.headers['Authorization']).toBeUndefined();
  });

  test('preserves existing headers when adding Authorization', () => {
    setTokenGetter(() => 'token123');
    const result = runRequest({ headers: { 'Content-Type': 'application/json' } });
    expect(result.headers['Authorization']).toBe('Bearer token123');
    expect(result.headers['Content-Type']).toBe('application/json');
  });

  test('returns config unchanged (no token, empty headers)', () => {
    setTokenGetter(() => null);
    const config = { headers: {}, url: '/api/students' };
    const result = runRequest(config);
    expect(result.url).toBe('/api/students');
    expect(Object.keys(result.headers)).toHaveLength(0);
  });
});

describe('axios response interceptor (Task 9.2)', () => {
  let originalLocation;

  beforeEach(() => {
    originalLocation = window.location.pathname;
    // jsdom pathname defaults to '/' which is not '/login', so 401 triggers redirect
    Object.defineProperty(window, 'location', {
      configurable: true,
      value: { pathname: '/students', href: '' },
    });
  });

  afterEach(() => {
    Object.defineProperty(window, 'location', {
      configurable: true,
      value: { pathname: originalLocation, href: '' },
    });
  });

  function runResponseError(status) {
    const handler = axiosInstance.interceptors.response.handlers[0];
    return handler.rejected({ response: { status } });
  }

  test('401 on a protected page sets location.href to /login', async () => {
    await expect(runResponseError(401)).rejects.toMatchObject({ response: { status: 401 } });
    expect(window.location.href).toBe('/login');
  });

  test('403 does NOT redirect — passes error through', async () => {
    window.location.href = '';
    await expect(runResponseError(403)).rejects.toMatchObject({ response: { status: 403 } });
    expect(window.location.href).toBe('');
  });

  test('500 does NOT redirect — passes error through', async () => {
    window.location.href = '';
    await expect(runResponseError(500)).rejects.toMatchObject({ response: { status: 500 } });
    expect(window.location.href).toBe('');
  });
});
