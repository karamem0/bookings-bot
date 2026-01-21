//
// Copyright (c) 2021-2026 karamem0
//
// This software is released under the MIT License.
//
// https://github.com/karamem0/bookings-bot/blob/main/LICENSE
//

import { BrowserCacheLocation } from '@azure/msal-browser';

export const msalConfig = {
  auth: {
    authority: import.meta.env.VITE_MICROSOFT_AUTHORITY,
    clientId: import.meta.env.VITE_MICROSOFT_CLIENT_APP_ID,
    redirectUri: `${window.location.origin}`
  },
  cache: {
    cacheLocation: BrowserCacheLocation.SessionStorage,
    storeAuthStateInCookie: false
  }
};

export const loginParams = {
  scopes: [
    `${import.meta.env.VITE_MICROSOFT_SERVER_APP_ID}/user_impersonation`
  ]
};
