# Variables
SOLUTION = $(CURDIR)/GeneracionNumeracionAvalonia.sln
TEST_PROJECT = $(CURDIR)/GeneracionNumeracionTest/GeneracionNumeracionTest.csproj
PACKAGE = ../../package/Macos/packaging.sh

# Tareas
restore:
	dotnet restore $(SOLUTION)

build:
	dotnet build $(SOLUTION)

format:
	dotnet format $(SOLUTION)

test: format 
	dotnet test $(TEST_PROJECT)

package:
	@echo "Ejecutando el script de empaquetado desde: $(PACKAGE)"
	sh $(PACKAGE)

all: restore build test package