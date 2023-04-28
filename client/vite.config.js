import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import viteTsconfigPaths from 'vite-tsconfig-paths';
import svgPlugin from 'vite-plugin-svgr';
export default defineConfig(() => {
  return {
    build: {
      outDir: 'build'
    },
    plugins: [react(), viteTsconfigPaths(), svgPlugin()]
  };
});
