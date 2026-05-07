//
// Copyright (c) 2021-2026 karamem0
//
// This software is released under the MIT License.
//
// https://github.com/karamem0/bookings-bot/blob/main/LICENSE
//

import React from 'react';

import Presenter from './RedirectPage.presenter';

import { broadcastResponseToMainFrame } from '@azure/msal-browser/redirect-bridge';
import { useError } from 'react-use';

function RedirectPage() {

  const dispatchError = useError();

  React.useEffect(() => {
    (async () => {
      try {
        await broadcastResponseToMainFrame();
      } catch (error) {
        if (error instanceof Error) {
          dispatchError(error);
        } else {
          console.error(error);
        }
      }
    })();
  }, [
    dispatchError
  ]);

  return (
    <Presenter />
  );

}

export default RedirectPage;
