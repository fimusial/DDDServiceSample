helm repo add bitnami https://charts.bitnami.com/bitnami
helm dependency build ./helm
helm install ddd-service-sample ./helm --set-file "postgresql.primary.initdb.scripts.init\.sql=./src/db-schema.postgres.sql"