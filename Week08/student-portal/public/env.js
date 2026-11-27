// Default runtime config for local dev (npm run dev / vite build).
// The Week10 Docker image overwrites this file at container start via
// Week10/web/env.sh so it can inject the gateway URL and route prefixes.
window.__ENV__ = {
  API_BASE_URL: '',
  IDENTITY_PREFIX: '',
  ACADEMICS_PREFIX: '',
  REPORTING_PREFIX: '',
};
