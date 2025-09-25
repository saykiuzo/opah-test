.PHONY: docker test-docker help clean stop create-migrations run

COMPOSE_FILE = docker-compose.yml
SOLUTION_FILE = FluxoCaixa.sln

help:
	@echo "Comandos disponíveis:"
	@echo "  make run             - Sobe tudo no Docker (com migrations automáticas)"
	@echo "  make create-migrations - Cria migrations se não existirem"
	@echo "  make test            - Executa testes no Docker"
	@echo "  make stop            - Para todos os serviços"
	@echo "  make clean           - Para todos os serviços e limpa volumes"

create-migrations:
	@cd src\Lancamentos\Lancamentos.Infrastructure && (if not exist "Migrations" (dotnet ef migrations add InitialCreate --startup-project ..\Lancamentos.API))
	@cd src\Consolidado\Consolidado.Infrastructure && (if not exist "Migrations" (dotnet ef migrations add InitialCreate --startup-project ..\Consolidado.API))

run: create-migrations
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