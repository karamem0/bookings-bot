//
// Copyright (c) 2021-2026 karamem0
//
// This software is released under the MIT License.
//
// https://github.com/karamem0/bookings-bot/blob/main/LICENSE
//

import React from 'react';

import Presenter from './Error404Page.presenter';

import { SeverityLevel } from '@microsoft/applicationinsights-common';
import { appInsights } from '../providers/TelemetryProvider';

interface Error500PageProps {
  error?: Error
}

function Error500Page(props: Readonly<Error500PageProps>) {

  const { error } = props;

  React.useEffect(() => {
    if (error == null) {
      return;
    }
    appInsights.trackException({ exception: error });
    appInsights.trackTrace({
      message: error.message,
      severityLevel: SeverityLevel.Critical
    });
  }, [
    error
  ]);

  return (
    <Presenter />
  );

}

export default Error500Page;
