import { cp } from "fs/promises";
import { fileURLToPath } from "url";
import { dirname, join } from "path";

const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

const curViteMap = join(__dirname, "../Riffle/Resources/viteMap.json");
const targetViteMap = join(__dirname, "../Riffle/bin/Debug/net9.0/Resources/viteMap.json");
await cp(curViteMap, targetViteMap);

console.log("Vite Map switched.");
