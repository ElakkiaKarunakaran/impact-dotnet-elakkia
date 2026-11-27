// Task 10.5 — path prefixes so the SAME React client can run either:
//   (a) standalone against Week07/08/09's single API (prefixes empty, unchanged behaviour), or
//   (b) against the Week 10 YARP gateway, which routes by prefix to each microservice.
// Read from window.__ENV__ (injected at container start — see Week10/docker-compose.yml
// and Week10/web/env.sh) so this file stays plain CommonJS-safe for Jest; default to ''
// so Week 8/9 behaviour and tests are completely unaffected.
const env = (typeof window !== 'undefined' && window.__ENV__) || {};

export const IDENTITY_PREFIX  = env.IDENTITY_PREFIX  || '';
export const ACADEMICS_PREFIX = env.ACADEMICS_PREFIX || '';
export const REPORTING_PREFIX = env.REPORTING_PREFIX || '';
