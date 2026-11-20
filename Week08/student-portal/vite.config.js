import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    proxy: {
      // Task 8.9 — proxy /api to the .NET API during dev (avoids CORS issues in dev)
      '/api': {
        target: 'http://localhost:5259',
        changeOrigin: true,
      },
    },
  },
});
