import { defineConfig } from "vite";
import { resolve } from "path";
import { renameSync } from "fs";

const outDir = "../Riffle/wwwroot/dist";

export default defineConfig({
    base: "/dist/",
    build: {
        outDir: resolve(__dirname, outDir),
        manifest: "viteMap.json",
        sourcemap: true,
        emptyOutDir: true,
        rollupOptions: {
            input: {
                roundaboutHost: resolve(__dirname, "src/host/roundaboutHost.ts")
            }
        }
    },
    cacheDir: resolve(__dirname, ".vite"),
    plugins: [
        {
            name: "move-manifest",
            async writeBundle() {
                const manifest = resolve(__dirname, outDir, "viteMap.json");
                const target = resolve(__dirname, "../Riffle/Resources/viteMap.json");

                renameSync(manifest, target);
            }
        }
    ]
});