#!/usr/bin/env bash
# Air cloud startup script for TeamCity.VSTest.TestAdapter (.NET project).
#
# Unit tests (TeamCity.VSTest.TestLogger.Tests) target net8.0 and reference the
# logger library (netstandard1.5 target), so the .NET 8 SDK is all a task needs.
# The full build.proj pipeline (net35/net40 publish + integration tests via
# vstest.console.exe) is Windows-only and is intentionally not run here.
set -euo pipefail

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
cd "$REPO_ROOT"

if [ "${AIR_STARTUP_MODE:-}" = warmup ]; then WARMUP=1; else WARMUP=; fi

log() { echo "[startup] $*"; }

# --- .NET SDK 8 ---------------------------------------------------------------
ensure_dotnet() {
  if command -v dotnet >/dev/null 2>&1 && dotnet --list-sdks 2>/dev/null | grep -q '^8\.'; then
    log ".NET SDK 8 already installed: $(dotnet --version)"
    return 0
  fi
  log "Installing .NET SDK 8.0 via dotnet-install.sh"
  curl -fsSL https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh
  bash /tmp/dotnet-install.sh --channel 8.0 --install-dir "$HOME/.dotnet"
  export DOTNET_ROOT="$HOME/.dotnet"
  export PATH="$HOME/.dotnet:$PATH"
  # Make dotnet available in future task shells too.
  if ! grep -q 'DOTNET_ROOT=' "$HOME/.bashrc" 2>/dev/null; then
    {
      echo 'export DOTNET_ROOT="$HOME/.dotnet"'
      echo 'export PATH="$HOME/.dotnet:$PATH"'
    } >> "$HOME/.bashrc"
  fi
  log "Installed: $(dotnet --version)"
}

ensure_dotnet

export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=1

# --- Warm caches (restore + build) --------------------------------------------
# Restoring the solution primes the NuGet cache for every project (library,
# unit tests, integration test projects). Building the unit test project also
# compiles the logger's netstandard1.5 target.
log "Restoring NuGet packages for the solution"
dotnet restore TeamCity.VSTest.TestAdapter.sln

log "Building unit test project (Release)"
dotnet build TeamCity.VSTest.TestLogger.Tests/TeamCity.VSTest.TestLogger.Tests.csproj -c Release

# --- Healthcheck ---------------------------------------------------------------
# Proves the environment works the way a real task needs it to: the .NET 8 SDK
# is usable and the unit test suite builds and passes.
healthcheck() {
  log "Healthcheck: running unit tests"
  dotnet test TeamCity.VSTest.TestLogger.Tests/TeamCity.VSTest.TestLogger.Tests.csproj -c Release --no-build
  log "Healthcheck passed"
}

if [ -n "${WARMUP:-}" ]; then
  healthcheck
fi

log "Startup complete"
