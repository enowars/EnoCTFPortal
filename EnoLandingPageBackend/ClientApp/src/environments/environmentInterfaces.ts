export interface EnvironmentInterface {
  /**
   * Determines if this is a production build or not.
   * Should only be used as a last resort for AoT-Compilation errors.
   */
  production: boolean;
  /**
   * Switch for activating Angular Route Tracing
   */
  routeTracing: boolean;
  /**
   * Whether Hot Module Reloading is enabled or not.
   */
  hmr: boolean;
  /**
   * The Backend Base Url
   */
  backendBaseUrl: string;
}
