#!/bin/sh
# Task 10.5 — writes window.__ENV__ at container START (not build time) so the
# same built image can be pointed at different gateway URLs via compose env vars.
# Runs as an nginx docker-entrypoint.d hook: it writes the file and returns,
# then the base image's own entrypoint proceeds to start nginx as usual.
set -e

cat > /usr/share/nginx/html/env.js <<EOF
window.__ENV__ = {
  API_BASE_URL: "${API_BASE_URL:-http://localhost:8080}",
  IDENTITY_PREFIX: "${IDENTITY_PREFIX:-/identity}",
  ACADEMICS_PREFIX: "${ACADEMICS_PREFIX:-/academics}",
  REPORTING_PREFIX: "${REPORTING_PREFIX:-/reporting}"
};
EOF
