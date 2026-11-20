import axiosInstance, { setTokenGetter } from '../api/axiosInstance';

// Task 8.7 — the axios interceptor must attach Authorization: Bearer <token>
// on every outgoing request when a token is present.

describe('axios interceptor', () => {
  afterEach(() => {
    // Reset the token getter to a no-op after each test
    setTokenGetter(() => null);
  });

  function runInterceptor(config = { headers: {} }) {
    // Directly invoke the interceptor's fulfilled handler
    const handler = axiosInstance.interceptors.request.handlers[0];
    return handler.fulfilled(config);
  }

  test('attaches Authorization: Bearer header when token is present', () => {
    setTokenGetter(() => 'my-jwt-token');
    const result = runInterceptor({ headers: {} });
    expect(result.headers['Authorization']).toBe('Bearer my-jwt-token');
  });

  test('does NOT add Authorization header when no token', () => {
    setTokenGetter(() => null);
    const result = runInterceptor({ headers: {} });
    expect(result.headers['Authorization']).toBeUndefined();
  });

  test('preserves existing headers when adding Authorization', () => {
    setTokenGetter(() => 'token123');
    const result = runInterceptor({ headers: { 'Content-Type': 'application/json' } });
    expect(result.headers['Authorization']).toBe('Bearer token123');
    expect(result.headers['Content-Type']).toBe('application/json');
  });

  test('returns config unchanged (no token, empty headers)', () => {
    setTokenGetter(() => null);
    const config = { headers: {}, url: '/api/students' };
    const result = runInterceptor(config);
    expect(result.url).toBe('/api/students');
    expect(Object.keys(result.headers)).toHaveLength(0);
  });
});
