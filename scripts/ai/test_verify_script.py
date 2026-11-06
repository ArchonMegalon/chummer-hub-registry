#!/usr/bin/env python3
"""Exercise verify.sh routing and failure handling with a stub, without running an SDK."""

import json
import os
from pathlib import Path
import subprocess
import tempfile
import unittest


class VerifyScriptChecks(unittest.TestCase):
    def setUp(self):
        self.temporary = tempfile.TemporaryDirectory(prefix="registry-script-")
        self.addCleanup(self.temporary.cleanup)
        self.root = Path(self.temporary.name)
        self.script = Path(__file__).resolve().with_name("verify.sh")
        self.repo = self.script.parents[2]
        tools = self.root / "stub sdk"
        tools.mkdir()
        dotnet = tools / "dotnet"
        dotnet.write_text(
            "#!/usr/bin/env python3\n"
            "import json, os, sys\n"
            "with open(os.environ['VERIFY_CALL_LOG'], 'a') as log:\n"
            "    log.write(json.dumps(sys.argv[1:]) + '\\n')\n"
            "sys.exit(17 if sys.argv[1] == os.environ.get('VERIFY_FAIL_COMMAND') else 0)\n"
        )
        dotnet.chmod(0o755)
        self.log = self.root / "calls.jsonl"
        self.consumer = self.root / "consumer with spaces"
        self.consumer.mkdir()
        self.env = {
            **os.environ,
            "PATH": str(tools) + os.pathsep + os.environ["PATH"],
            "DOTNET_CLI_HOME": str(self.root / "cli"),
            "NUGET_PACKAGES": str(self.root / "nuget"),
            "CHUMMER_REGISTRY_PACK_OUTPUT": str(self.root / "package output"),
            "VERIFY_CALL_LOG": str(self.log),
        }
        self.env.pop("CHUMMER_RUN_SERVICES_ROOT", None)
        self.env.pop("VERIFY_FAIL_COMMAND", None)

    def run_script(self, *arguments):
        return subprocess.run(
            ["bash", str(self.script), *arguments],
            cwd=self.root,
            env=self.env,
            text=True,
            capture_output=True,
            check=False,
        )

    def calls(self):
        return [json.loads(line) for line in self.log.read_text().splitlines()] if self.log.exists() else []

    def test_missing_and_invalid_consumer_fail_before_sdk(self):
        for consumer in (None, str(self.root / "missing")):
            with self.subTest(consumer=consumer):
                if consumer is not None:
                    self.env["CHUMMER_RUN_SERVICES_ROOT"] = consumer
                result = self.run_script()
                self.assertEqual(2, result.returncode)
                self.assertIn("CHUMMER_RUN_SERVICES_ROOT", result.stderr)
                self.assertEqual([], self.calls())

    def test_invalid_options_fail_before_sdk(self):
        for args in (("--unknown",), ("--contracts-only", "extra")):
            with self.subTest(args=args):
                result = self.run_script(*args)
                self.assertEqual(2, result.returncode)
                self.assertIn("Usage:", result.stderr)
                self.assertEqual([], self.calls())

    def test_contracts_only_uses_current_checkout_and_reuses_build(self):
        result = self.run_script("--contracts-only")
        self.assertEqual(0, result.returncode, result.stderr)
        verifier = str(self.repo / "Chummer.Hub.Registry.Contracts.Verify/Chummer.Hub.Registry.Contracts.Verify.csproj")
        contracts = str(self.repo / "Chummer.Hub.Registry.Contracts/Chummer.Hub.Registry.Contracts.csproj")
        self.assertEqual(
            [
                ["build", verifier, "-c", "Debug"],
                ["run", "--project", verifier, "-c", "Debug", "--no-build", "--no-restore", "--", "--contracts-only"],
                ["pack", contracts, "--no-build", "--no-restore", "-c", "Debug", "-o", self.env["CHUMMER_REGISTRY_PACK_OUTPUT"]],
            ],
            self.calls(),
        )

    def test_consumer_mode_does_not_pass_skip_flag(self):
        self.env["CHUMMER_RUN_SERVICES_ROOT"] = str(self.consumer)
        result = self.run_script()
        self.assertEqual(0, result.returncode, result.stderr)
        self.assertEqual("--", self.calls()[1][-1])
        self.assertNotIn("--contracts-only", self.calls()[1])

    def test_failed_build_or_verifier_stops_before_pack(self):
        for failure, count in (("build", 1), ("run", 2)):
            with self.subTest(failure=failure):
                self.log.unlink(missing_ok=True)
                self.env["VERIFY_FAIL_COMMAND"] = failure
                result = self.run_script("--contracts-only")
                self.assertEqual(17, result.returncode)
                self.assertEqual(count, len(self.calls()))
                self.assertNotIn("pack", [call[0] for call in self.calls()])


if __name__ == "__main__":
    unittest.main()
