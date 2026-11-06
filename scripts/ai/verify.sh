#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")/../.." && pwd)"
if (( $# > 1 )) || { (( $# == 1 )) && [[ "$1" != "--contracts-only" ]]; }; then
    echo "Usage: verify.sh [--contracts-only]" >&2
    exit 2
fi
if (( $# == 0 )) && [[ -z "${CHUMMER_RUN_SERVICES_ROOT:-}" || ! -d "$CHUMMER_RUN_SERVICES_ROOT" ]]; then
    echo "Set CHUMMER_RUN_SERVICES_ROOT to the consumer checkout, or use --contracts-only (ownership NOT checked)." >&2
    exit 2
fi

export DOTNET_CLI_HOME="${DOTNET_CLI_HOME:-/tmp/dotnet-cli-home}"
export NUGET_PACKAGES="${NUGET_PACKAGES:-/tmp/nuget-packages}"

mkdir -p "$DOTNET_CLI_HOME" "$NUGET_PACKAGES"

verifier_project="$repo_root/Chummer.Hub.Registry.Contracts.Verify/Chummer.Hub.Registry.Contracts.Verify.csproj"
contracts_project="$repo_root/Chummer.Hub.Registry.Contracts/Chummer.Hub.Registry.Contracts.csproj"
dotnet build "$verifier_project" -c Debug
dotnet run --project "$verifier_project" -c Debug --no-build --no-restore -- "$@"
pack_output="${CHUMMER_REGISTRY_PACK_OUTPUT:-$(mktemp -d "${TMPDIR:-/tmp}/chummer-hub-registry-pack.XXXXXX")}"
dotnet pack "$contracts_project" --no-build --no-restore -c Debug -o "$pack_output"
echo "Registry package output: $pack_output"
