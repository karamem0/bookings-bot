//
// Copyright (c) 2021-2026 karamem0
//
// This software is released under the MIT License.
//
// https://github.com/karamem0/bookings-bot/blob/main/LICENSE
//

import React from 'react';

import { css } from '@emotion/react';
import { Spinner } from '@fluentui/react-components';
import { useIntl } from 'react-intl';
import messages from '../messages';

function RedirectPage() {

  const intl = useIntl();

  return (
    <React.Fragment>
      <meta
        content={intl.formatMessage(messages.AppCreator)}
        name="author" />
      <meta
        content={intl.formatMessage(messages.AppDescription)}
        name="description" />
      <title>
        {intl.formatMessage(messages.AppTitle)}
      </title>
      <div
        css={css`
          display: flex;
          flex-direction: column;
          align-items: center;
          justify-content: center;
          min-height: 100svh;
        `}>
        <Spinner />
      </div>
    </React.Fragment>
  );

}

export default RedirectPage;
