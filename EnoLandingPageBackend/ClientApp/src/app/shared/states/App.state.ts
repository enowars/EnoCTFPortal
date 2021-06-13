import { Injectable } from '@angular/core';
import { Action, Selector, State, StateContext } from '@ngxs/store';
import { ThemeService } from 'src/app/services/theme.service';
import { Theme } from 'src/app/shared/models/enumberables/Theme';

export class ServiceWorkerNotificationDisplayed {
  public static readonly type: string =
    '[App State] Service Worker Notification displayed';
}

export class SetVersion {
  public static readonly type: string = '[App State] Set Version';
  constructor(public version: string) {}
}
export class SetDisplayedNotificationVersion {
  public static readonly type: string =
    '[App State] Set Displayed Notification Version';
  constructor(public version: number) {}
}

export class SetTheme {
  public static readonly type: string = '[App State] Set activeTheme';
  constructor(public activeTheme: Theme) {}
}

export class InitTheme {
  public static readonly type: string = '[App State] Init activeTheme';
}

export interface AppStateModel {
  serviceWorkerNotificationDisplayed: boolean;
  version: string;
  activeTheme: Theme;
}

@State<AppStateModel>({
  name: 'appstate',
  defaults: {
    serviceWorkerNotificationDisplayed: false,
    version: '0.0.0',
    activeTheme: Theme.default_dark,
  },
})
@Injectable()
export class AppState {
  constructor(private themeService: ThemeService) {}
  @Selector()
  public static serviceWorkerNotificationDisplayed(
    state: AppStateModel
  ): boolean {
    return state.serviceWorkerNotificationDisplayed;
  }

  @Selector()
  public static version(state: AppStateModel): string {
    return state.version;
  }
  @Selector()
  public static activeTheme(state: AppStateModel): Theme {
    return state.activeTheme;
  }

  @Action(ServiceWorkerNotificationDisplayed)
  public serviceWorkerNotificationDisplayed(ctx: StateContext<AppStateModel>) {
    const state = ctx.getState();
    ctx.setState({
      ...state,
      serviceWorkerNotificationDisplayed: true,
    });
  }

  @Action(SetVersion)
  public setVersion(ctx: StateContext<AppStateModel>, action: SetVersion) {
    const state = ctx.getState();
    ctx.setState({
      ...state,
      version: action.version,
    });
  }

  @Action(SetTheme)
  public setTheme(ctx: StateContext<AppStateModel>, action: SetTheme) {
    const state = ctx.getState();
    this.themeService.setTheme(action.activeTheme);
    ctx.setState({
      ...state,
      activeTheme: action.activeTheme,
    });
  }

  @Action(InitTheme)
  public initTheme(ctx: StateContext<AppStateModel>) {
    const state = ctx.getState();
    this.themeService.setTheme(state.activeTheme);
  }
}
