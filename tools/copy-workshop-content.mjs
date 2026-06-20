import { cp, mkdir, stat } from "node:fs/promises";
import { basename, dirname, resolve } from "node:path";
import { fileURLToPath } from "node:url";

const VALID_CONFIGURATIONS = new Set(["Debug", "ExportRelease"]);

function getConfiguration() {
  const configuration = process.argv[2] ?? "Debug";

  if (!VALID_CONFIGURATIONS.has(configuration)) {
    throw new Error(
      `Invalid configuration "${configuration}". Expected one of: ${Array.from(VALID_CONFIGURATIONS).join(", ")}.`,
    );
  }

  return configuration;
}

async function assertFileExists(filePath) {
  let fileStats;

  try {
    fileStats = await stat(filePath);
  } catch {
    throw new Error(`Required file not found: ${filePath}`);
  }

  if (!fileStats.isFile()) {
    throw new Error(`Expected a file but found something else: ${filePath}`);
  }
}

const repoRoot = resolve(dirname(fileURLToPath(import.meta.url)), "..");
const configuration = getConfiguration();
const buildDir = resolve(
  repoRoot,
  ".godot",
  "mono",
  "temp",
  "bin",
  configuration,
);
const destinationDir = resolve(
  repoRoot,
  "dev",
  "ModUploader-win-x64",
  "LinkuraMod",
  "content",
);

const filesToCopy = [
  resolve(buildDir, "LinkuraMod.dll"),
  resolve(buildDir, "LinkuraMod.pdb"),
  resolve(repoRoot, "mod_manifest.json"),
  resolve(repoRoot, "LinkuraMod.pck"),
];

await mkdir(destinationDir, { recursive: true });

for (const filePath of filesToCopy) {
  await assertFileExists(filePath);
  await cp(filePath, resolve(destinationDir, basename(filePath)));
}

console.log(
  `Copied build artifacts from ${configuration} to ${destinationDir}`,
);
