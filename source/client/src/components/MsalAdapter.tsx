//
// Copyright (c) 2021-2026 karamem0
//
// This software is released under the MIT License.
//
// https://github.com/karamem0/bookings-bot/blob/main/LICENSE
//

import React from 'react';

import { InteractionStatus } from '@azure/msal-browser';
import { useMsal } from '@azure/msal-react';
import Presenter from './MsalAdapter.presenter';

function MsalAdapter(props: Readonly<React.PropsWithChildren<unknown>>) {

  const { children } = props;

  const msal = useMsal();

  return (
    <Presenter loading={msal.inProgress !== InteractionStatus.None}>
      {children}
    </Presenter>
  );

}

export default MsalAdapter;
