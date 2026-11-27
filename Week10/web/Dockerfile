FROM node:20-alpine AS build
WORKDIR /app
COPY Week08/student-portal/package*.json ./
RUN npm ci
COPY Week08/student-portal/ ./
RUN npm run build

FROM nginx:1.27-alpine AS runtime
COPY --from=build /app/dist /usr/share/nginx/html
COPY Week10/web/nginx.conf /etc/nginx/conf.d/default.conf
COPY Week10/web/env.sh /docker-entrypoint.d/40-env.sh
RUN chmod +x /docker-entrypoint.d/40-env.sh
EXPOSE 80
