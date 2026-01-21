//
// Copyright (c) 2021-2026 karamem0
//
// This software is released under the MIT License.
//
// https://github.com/karamem0/bookings-bot/blob/main/LICENSE
//

import React from 'react';

import {
  Button,
  Link,
  Text
} from '@fluentui/react-components';
import { FormattedMessage, useIntl } from 'react-intl';
import { GrGithub, GrWindows } from 'react-icons/gr';
import { EventHandler } from '../types/Event';
import { css } from '@emotion/react';
import messages from '../messages';
import { useTheme } from '../providers/ThemeProvider';

interface HomePageProps {
  onLinkClick?: EventHandler<string>,
  onSignIn?: EventHandler
}

function HomePage(props: Readonly<HomePageProps>) {

  const {
    onLinkClick,
    onSignIn
  } = props;

  const intl = useIntl();
  const { theme } = useTheme();

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
          flex-flow: column;
          min-width: 100%;
          min-height: 100svh;
        `}>
        <header
          css={css`
            display: flex;
            flex-flow: row;
            align-items: center;
            justify-content: end;
            min-height: 2.5rem;
            padding: 0 1rem;
            background-color: ${theme.colorNeutralBackgroundInverted};
          `}>
          <Button
            appearance="transparent"
            as="a"
            css={css`
              color: ${theme.colorNeutralForegroundInverted};
            `}
            icon={(
              <GrGithub
                css={css`
                  font-size: 1rem;
                  line-height: 1rem;
                `} />
            )}
            onClick={(event) => onLinkClick?.(event, 'GitHub')}>
            <FormattedMessage {...messages.GitHub} />
          </Button>
        </header>
        <main
          css={css`
            display: flex;
            flex-flow: column;
            min-height: calc(100svh - 5rem);
            background: linear-gradient(${theme.colorBrandBackground2Hover}, ${theme.colorBrandBackground2});
          `}>
          <div
            css={css`
              display: flex;
              flex-flow: column;
              gap: 4rem;
              padding: 4rem 0;
            `}>
            <div
              css={css`
                display: flex;
                flex-flow: column;
                gap: 1rem;
                align-items: center;
                justify-content: center;
              `}>
              <Text
                as="h1"
                css={css`
                  font-size: ${theme.fontSizeHero900};
                  font-weight: bold;
                  line-height: calc(${theme.fontSizeHero900} * 1.25);
                  color: ${theme.colorNeutralForeground2};
                `}>
                <FormattedMessage {...messages.AppTitle} />
              </Text>
              <Text
                css={css`
                  font-size: ${theme.fontSizeBase300};
                  font-weight: bold;
                  line-height: calc(${theme.fontSizeBase300} * 1.25);
                  color: ${theme.colorNeutralForeground2};
                `}>
                <FormattedMessage {...messages.AppDescription} />
              </Text>
            </div>
            <div
              css={css`
                display: flex;
                flex-flow: column;
                align-items: center;
                justify-content: center;
              `}>
              <Button
                appearance="primary"
                as="a"
                icon={(
                  <GrWindows
                    css={css`
                      font-size: 1rem;
                      line-height: 1rem;
                    `} />
                )}
                onClick={onSignIn}>
                <FormattedMessage {...messages.SignIn} />
              </Button>
            </div>
          </div>
        </main>
        <footer
          css={css`
            display: flex;
            flex-flow: row;
            align-items: center;
            justify-content: center;
            min-height: 2.5rem;
            padding: 0 1rem;
          `}>
          <Link
            as="button"
            onClick={(event) => onLinkClick?.(event, 'TermsOfUse')}>
            <FormattedMessage {...messages.TermsOfUse} />
          </Link>
          <Text
            css={css`
              padding: 0 0.25rem;
            `}>
            |
          </Text>
          <Link
            as="button"
            onClick={(event) => onLinkClick?.(event, 'PrivacyPolicy')}>
            <FormattedMessage {...messages.PrivacyPolicy} />
          </Link>
        </footer>
      </div>
    </React.Fragment>
  );

}

export default React.memo(HomePage);
