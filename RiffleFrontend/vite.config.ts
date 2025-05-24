import { defineConfig } from "vite";
import { resolve } from "path";

export default defineConfig({
    build: {
        outDir: "../Riffle/wwwroot/dist",
        emptyOutDir: true,
        rollupOptions: {
            input: {
                roundaboutHost: resolve(__dirname, "src/roundaboutHost.ts")
            }
        }
    }
});