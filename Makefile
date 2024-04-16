# Variables
.PHONY: package # Ambiguous reference with package folder
SOLUTION = $(CURDIR)/GeneracionNumeracionAvalonia.sln
TEST_PROJECT = $(CURDIR)/GeneracionNumeracionTest/GeneracionNumeracionTest.csproj
PACKAGE_MAC = $(CURDIR)/package/Macos/packaging.sh
PACKAGE_WIN = $(CURDIR)/package/Win/packaging.sh
# Tareas
restore:
	dotnet restore $(SOLUTION)
	@echo $(PACKAGE)

build:
	dotnet build $(SOLUTION)

format:
	dotnet format $(SOLUTION)

test: format 
	dotnet test $(TEST_PROJECT)

package:
	@echo "Generation Mac Os package"
	sh $(PACKAGE_MAC)
	@echo "Generation Win package"
	sh $(PACKAGE_WIN)

all: restore build test package