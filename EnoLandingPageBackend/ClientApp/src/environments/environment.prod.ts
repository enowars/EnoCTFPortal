import { EnvironmentInterface } from './environmentInterfaces';

export const environment: EnvironmentInterface = {
  production: true,
  hmr: false,
  // no string is the base from where the app is currently served
  backendBaseUrl: '',
  routeTracing: false,
};
