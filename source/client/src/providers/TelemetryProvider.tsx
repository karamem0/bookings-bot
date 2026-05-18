//
// Copyright (c) 2021-2026 karamem0
//
// This software is released under the MIT License.
//
// https://github.com/karamem0/bookings-bot/blob/main/LICENSE
//

import React from 'react';

import {
  ReactPlugin,
  withAITracking
} from '@microsoft/applicationinsights-react-js';
import { ApplicationInsights } from '@microsoft/applicationinsights-web';
import { telemetryConfig } from '../config/TelemetryConfig';

export const reactPlugin = new ReactPlugin();

export const appInsights = new ApplicationInsights({
  config: {
    ...telemetryConfig,
    extensions: [
      reactPlugin
    ]
  }
});

try {
  appInsights.loadAppInsights();
  appInsights.trackPageView();
} catch (error) {
  console.error(error);
}

function TelemetryProvider(props: React.PropsWithChildren<unknown>) {

  const { children } = props;

  return (
    <React.Fragment>
      {children}
    </React.Fragment>
  );

}

export default withAITracking(reactPlugin, TelemetryProvider);
