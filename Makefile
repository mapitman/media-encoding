.PHONY: help venv install clean activate rip-tv

# Use bash for all recipe commands
SHELL := /bin/bash

# Default Python version
PYTHON := python3
VENV := venv
BIN := $(VENV)/bin
CURRENT_SHELL := $(shell basename $$SHELL)

# Defaults for rip targets (override via: make rip-movie OUTPUT=... EXTRA_ARGS="--title Foo")
OUTPUT ?= $(HOME)/Videos
EXTRA_ARGS ?=

help:
	@echo "Available targets:"
	@echo "  make venv      - Create virtual environment"
	@echo "  make install   - Install dependencies in virtual environment"
	@echo "  make activate  - Start a shell with virtual environment activated"
	@echo "  make rip-movie - Activate venv and run rip_movie.sh (use OUTPUT=/path and EXTRA_ARGS)"
	@echo "  make rip-tv    - Activate venv and run rip_tv.sh (use OUTPUT=/path and EXTRA_ARGS)"
	@echo "  make clean     - Remove virtual environment"
	@echo "  make all       - Create venv and install dependencies"

venv:
	@echo "Creating virtual environment..."
	$(PYTHON) -m venv $(VENV)
	@echo "Virtual environment created in $(VENV)/"
	@echo "Activate with: source $(BIN)/activate"

install: venv
	@echo "Installing dependencies..."
	$(BIN)/pip install --upgrade pip
	$(BIN)/pip install -r requirements.txt
	@echo "Dependencies installed successfully!"

activate: venv
	@echo "Starting $(CURRENT_SHELL) with virtual environment activated..."
	@$(BIN)/python -c "import sys; print(f'Python {sys.version}')"
	@echo "Virtual environment is active. Type 'exit' to return."
	@if [ "$(CURRENT_SHELL)" = "zsh" ]; then \
		zsh -i -c ". $(BIN)/activate; exec zsh -i"; \
	elif [ "$(CURRENT_SHELL)" = "bash" ]; then \
		bash -i -c ". $(BIN)/activate; exec bash -i"; \
	else \
		sh -i -c ". $(BIN)/activate; exec sh -i"; \
	fi


all: install

rip-movie: venv
	@echo "Ripping movie to $(OUTPUT)..."
	@. $(BIN)/activate; ./rip_movie.sh --output "$(OUTPUT)" $(EXTRA_ARGS)

rip-tv: venv
	@echo "Ripping TV to $(OUTPUT)..."
	@. $(BIN)/activate; ./rip_tv.sh --output "$(OUTPUT)" $(EXTRA_ARGS)

clean:
	@echo "Removing virtual environment..."
	rm -rf $(VENV)
	@echo "Virtual environment removed."
