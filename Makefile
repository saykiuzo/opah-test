.PHONY: docker test-docker help clean stop

COMPOSE_FILE = docker-compose.yml
SOLUTION_FILE = FluxoCaixa.sln

help:
	@echo "Comandos disponíveis:"
	@echo "  make run      - Sobe tudo no Docker (infraestrutura + APIs)"
	@echo "  make test     - Executa testes no Docker"
	@echo "  make stop     - Para todos os serviços"
	@echo "  make clean    - Para todos os serviços e limpa volumes"

run:
	docker-compose -f $(COMPOSE_FILE) build
	docker-compose -f $(COMPOSE_FILE) up -d
	@powershell -Command "Start-Sleep -Seconds 30"
	docker-compose -f $(COMPOSE_FILE) ps

test:
	docker build -t fluxocaixa-tests -f Dockerfile.tests .
	docker run --rm fluxocaixa-tests dotnet test FluxoCaixa.sln --verbosity normal

stop:
	docker-compose -f $(COMPOSE_FILE) down

clean:
	docker-compose -f $(COMPOSE_FILE) down -v
	docker system prune -f