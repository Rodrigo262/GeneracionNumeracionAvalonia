# Variables
.PHONY: package # Ambiguous reference with package folder
SOLUTION = $(CURDIR)/GeneracionNumeracionAvalonia.sln
TEST_PROJECT = $(CURDIR)/GeneracionNumeracionTest/GeneracionNumeracionTest.csproj
PACKAGE = $(CURDIR)/package/Macos/packaging.sh

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
	sh $(PACKAGE)

all: restore build test package