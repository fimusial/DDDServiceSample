$db = docker ps --format '{{.ID}}' --filter "publish=5432"
#$rabbitMQ = docker ps --format '{{.ID}}' --filter "publish=5672"

watch 2 "docker exec -it $db psql -U postgres -d application -c 'select * from public.memo; select * from public.integrationeventoutbox;'" -clear