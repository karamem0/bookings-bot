//
// Copyright (c) 2021-2026 karamem0
//
// This software is released under the MIT License.
//
// https://github.com/karamem0/bookings-bot/blob/main/LICENSE
//

import React from 'react';
import { createRoot } from 'react-dom/client';

import { AuthenticatedTemplate, UnauthenticatedTemplate } from '@azure/msal-react';
import { Global } from '@emotion/react';
import { ErrorBoundary } from 'react-error-boundary';
import {
  BrowserRouter,
  Route,
  Routes
} from 'react-router';
import * as ress from 'ress';
import MsalAdapter from './components/MsalAdapter';
import Error404Page from './pages/Error404Page';
import Error500Page from './pages/Error500Page';
import HomePage from './pages/HomePage';
import MainPage from './pages/MainPage';
import RedirectPage from './pages/RedirectPage';
import IntlProvider from './providers/IntlProvider';
import MsalProvider from './providers/MsalProvider';
import ThemeProvider from './providers/ThemeProvider';

const rootElement = document.getElementById('root');

if (rootElement == null) {
  throw new Error('Root element not found');
}

const root = createRoot(rootElement);

root.render(
  <React.Fragment>
    <Global styles={ress} />
    <BrowserRouter>
      <ThemeProvider>
        <IntlProvider>
          <ErrorBoundary
            fallbackRender={(props) => (
              <Error500Page error={props.error as Error} />
            )}>
            <Routes>
              <Route
                path="/"
                element={(
                  <MsalProvider>
                    <MsalAdapter>
                      <AuthenticatedTemplate>
                        <MainPage />
                      </AuthenticatedTemplate>
                      <UnauthenticatedTemplate>
                        <HomePage />
                      </UnauthenticatedTemplate>
                    </MsalAdapter>
                  </MsalProvider>
                )} />
              <Route
                element={<RedirectPage />}
                path="/auth_redirect" />
              <Route
                element={<Error404Page />}
                path="*" />
            </Routes>
          </ErrorBoundary>
        </IntlProvider>
      </ThemeProvider>
    </BrowserRouter>
  </React.Fragment>
);
