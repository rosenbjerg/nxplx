FROM redis:5.0.5-alpine

WORKDIR "/redis-5.0.5"
COPY /redis.conf /redis-5.0.5

CMD [ "redis-server", "redis.conf" ]