/* tslint:disable:no-invalid-template-strings */

export enum Environment {
  /**
   * Local or pr-preview environment - unstable.
   */
  development = 'development',
  /**
   * An Environment that might have Bugs and has Preview Features enabled by default
   * e.g. Early adopters or Staging Environment
   */
  preview = 'preview',
  /**
   * An Environment that is stable.
   * Preview features can be enabled by the user himself.
   */
  production = 'production',
}
export interface RuntimeEnvironmentInterface {
  /**
   * The Environment Tag is displayed right next to the title of the page, e.g. "preview", "dev"
   */
  readonly environmentTag: string;
  /**
   * The Environment the app is living in, by default it is development
   */
  readonly environment: Environment;
  /**
   * The Base Url of the RocketChat Messenger
   */
  readonly staticHosting?: string;
}

declare const ENV: RuntimeEnvironmentInterface;
export const runtimeEnvironment: RuntimeEnvironmentInterface = {
  environment:
    ENV.environment.toString() !== '${ENVIRONMENT}'
      ? ENV.environment
      : Environment.development,
  environmentTag:
    ENV.environmentTag !== '${ENVIRONMENT_TAG}' ? ENV.environmentTag : '',
  staticHosting:
    ENV.staticHosting !== '${STATIC_HOSTING}' ? ENV.staticHosting : 'false',
};
