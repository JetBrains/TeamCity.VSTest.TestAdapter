#!/usr/bin/env bash
# Air cloud startup script for TeamCity.VSTest.TestAdapter (.NET project).
#
# Unit tests (TeamCity.VSTest.TestLogger.Tests) target net8.0 and reference the
# logger library (net35/net40/netstandard1.5), so the .NET 8 SDK is all a task
# needs; the whole solution builds on Linux. The full build.proj pipeline
# (vstest.console.exe integration tests) is Windows-only and is not run here.
set -euo pipefail

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
cd "$REPO_ROOT"

if [ "${AIR_STARTUP_MODE:-}" = warmup ]; then WARMUP=1; else WARMUP=; fi

log() { echo "[startup] $*"; }

# --- libicu (required by the .NET runtime) -------------------------------------
ensure_libicu() {
  if ldconfig -p 2>/dev/null | grep -q 'libicuuc\.so'; then
    log "libicu already present"
    return 0
  fi
  log "Installing libicu"
  sudo apt-get update -qq
  local pkg
  pkg="$(apt-cache search --names-only '^libicu[0-9]+$' | awk '{print $1}' | sort -V | tail -1)"
  sudo apt-get install -y -qq "${pkg:-libicu-dev}"
}

# --- .NET SDK 8 ----------------------------------------------------------------
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
  log "Installed .NET SDK: $(dotnet --version)"
}

ensure_libicu
ensure_dotnet

export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=1

# --- Warm caches (restore + build) ----------------------------------------------
log "Restoring NuGet packages for the solution"
dotnet restore TeamCity.VSTest.TestAdapter.sln

log "Building the solution (Release)"
dotnet build TeamCity.VSTest.TestAdapter.sln -c Release --no-restore

# --- Healthcheck -----------------------------------------------------------------
# Proves the environment works the way a real task needs it to: the .NET 8 SDK
# is usable and the unit test suite builds and passes. IntegrationTests,
# SuiteNameProviderTests and AttachmentsTests are excluded: they depend on
# Windows path semantics / the Windows-only build.proj pipeline and fail on
# Linux by design, not because the environment is broken.
healthcheck() {
  log "Healthcheck: running Linux-compatible unit tests"
  dotnet test TeamCity.VSTest.TestLogger.Tests/TeamCity.VSTest.TestLogger.Tests.csproj \
    -c Release --no-build \
    --filter "FullyQualifiedName!~IntegrationTests&FullyQualifiedName!~SuiteNameProviderTests&FullyQualifiedName!~AttachmentsTests"
  log "Healthcheck passed"
}

if [ -n "${WARMUP:-}" ]; then
  healthcheck
fi

log "Startup complete"
