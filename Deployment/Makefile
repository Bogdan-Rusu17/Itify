REGISTRY = bogdanr17
DB_SERVICE_IMAGE = $(REGISTRY)/itify-db-service
AUTH_SERVICE_IMAGE = $(REGISTRY)/itify-auth
BUSINESS_SERVICE_IMAGE = $(REGISTRY)/itify-business

build-all:
	docker build -t $(DB_SERVICE_IMAGE) ../Itify.DbService/
	docker build -t $(AUTH_SERVICE_IMAGE) ../Itify.AuthService/
	docker build -t $(BUSINESS_SERVICE_IMAGE) ../Itify.BusinessService/

push-all:
	docker push $(DB_SERVICE_IMAGE)
	docker push $(AUTH_SERVICE_IMAGE)
	docker push $(BUSINESS_SERVICE_IMAGE)

cluster-up:
	kind create cluster --name local-itify-cluster --config=kind-config.yaml

cluster-down:
	kind delete cluster --name local-itify-cluster

deploy:                                                                              
	kubectl apply -f charts/namespace.yaml                                         
	kubectl apply -f charts/secrets/                                               
	kubectl apply -f charts/network-policies/                                      
	kubectl apply -f charts/postgres/
	kubectl apply -f charts/services/db-service.yaml
	kubectl apply -f charts/services/auth-service.yaml
	kubectl apply -f charts/services/business-service.yaml
	kubectl apply -f charts/services/pgadmin.yaml
	kubectl apply -f charts/services/grafana-external.yaml
	$(MAKE) install-kong
	$(MAKE) install-monitoring
	kubectl apply -f charts/kong/routes.yaml

install-kong:
	helm repo add kong https://charts.konghq.com
	helm repo update
	helm upgrade --install kong kong/ingress -n kong --create-namespace -f charts/kong/values.yaml

install-monitoring:
	helm repo add prometheus-community https://prometheus-community.github.io/helm-charts
	helm repo update
	helm upgrade --install monitoring prometheus-community/kube-prometheus-stack -n monitoring --create-namespace -f charts/monitoring/values.yaml


