//
// Copyright (c) 2021-2026 karamem0
//
// This software is released under the MIT License.
//
// https://github.com/karamem0/bookings-bot/blob/main/LICENSE
//

import react from '@vitejs/plugin-react-swc';
import fs from 'fs';
import { defineConfig } from 'vite';

export default defineConfig({
  'build': {
    'outDir': 'build',
    'sourcemap': true
  },
  'plugins': [
    react({
      'jsxImportSource': '@emotion/react',
      'plugins': [
        [
          '@swc/plugin-emotion',
          {
          }
        ],
        [
          '@swc/plugin-formatjs',
          {
            'ast': true,
            'idInterpolationPattern': '[sha512:contenthash:base64:6]'
          }
        ]
      ]
    })
  ],
  'server': {
    'https': {
      'cert': fs.readFileSync('./cert/localhost.crt'),
      'key': fs.readFileSync('./cert/localhost.key')
    },
    'proxy': {
      '/api': {
        'changeOrigin': true,
        'secure': false,
        'target': 'http://localhost:3978'
      }
    }
  }
});
