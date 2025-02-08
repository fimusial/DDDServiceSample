$db = docker ps --format '{{.ID}}' --filter "name=dddservicesample-db-1"
$rabbitMQ = docker ps --format '{{.ID}}' --filter "name=dddservicesample-broker-1"
#docker exec -it $db psql -U postgres -d application -c 'START TRANSACTION READ WRITE; delete from public.memo; delete from public.integration_event_outbox; COMMIT;'
watch 2 "docker exec -it $db psql -U postgres -d application -c 'select * from public.memo; select * from public.integration_event_outbox;'" -clear