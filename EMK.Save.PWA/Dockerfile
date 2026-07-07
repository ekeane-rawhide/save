# Build context must be the repo root:
#   docker build -f EMK.Save.PWA/Dockerfile -t save-pwa \
#     --build-arg VITE_API_URL=https://api.yourdomain.com/api \
#     --build-arg VITE_HUB_URL=https://api.yourdomain.com/hubs/save .
#
# VITE_* values are baked into the static bundle at build time — there is no runtime
# config for a plain static/nginx deploy, so rebuild the image if the API URL changes.

# ── Build ────────────────────────────────────────────────────────────────────
FROM node:22-alpine AS build
WORKDIR /app

COPY EMK.Save.PWA/package.json EMK.Save.PWA/package-lock.json ./
RUN npm ci

COPY EMK.Save.PWA/ ./

ARG VITE_API_URL
ARG VITE_HUB_URL
ARG VITE_VAPID_PUBLIC_KEY
ENV VITE_API_URL=${VITE_API_URL} \
    VITE_HUB_URL=${VITE_HUB_URL} \
    VITE_VAPID_PUBLIC_KEY=${VITE_VAPID_PUBLIC_KEY}

RUN npm run build

# ── Runtime ──────────────────────────────────────────────────────────────────
FROM nginx:1.27-alpine AS runtime

COPY --from=build /app/dist /usr/share/nginx/html
COPY EMK.Save.PWA/nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
    CMD wget -qO- http://localhost/ || exit 1
