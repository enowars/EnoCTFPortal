import { Injectable } from '@angular/core';
import {
  Action,
  NgxsOnInit,
  Selector,
  State,
  StateContext,
  Store,
} from '@ngxs/store';
import {
  AccountService,
  CtfInfoMessage,
  DataService,
} from 'projects/backend-api/src/lib';
import { TeamDetailsMessage } from 'projects/backend-api/src/lib/model/teamDetailsMessage';
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

export class Login {
  public static readonly type: string = '[App State] Login';
}

export interface AppStateModel {
  serviceWorkerNotificationDisplayed: boolean;
  version: string;
  activeTheme: Theme;
  authenticated: boolean;
  teamInfo: TeamDetailsMessage | null;
  ctfInfo: CtfInfoMessage | null;
}

@State<AppStateModel>({
  name: 'appstate',
  defaults: {
    serviceWorkerNotificationDisplayed: false,
    version: '0.0.0',
    activeTheme: Theme.default_dark,
    authenticated: false,
    teamInfo: null,
    ctfInfo: null,
  },
})
@Injectable()
export class AppState implements NgxsOnInit {
  constructor(
    private themeService: ThemeService,
    private accountService: AccountService,
    private dataService: DataService
  ) {}
  ngxsOnInit(ctx: StateContext<AppStateModel>) {
    this.accountService.apiAccountInfoGet().subscribe(
      (accountInfo) => {
        let state = ctx.getState();
        ctx.setState({ ...state, authenticated: true, teamInfo: accountInfo });
      },
      (error) => {
        // Do nothing the use is simply not authenticated
      }
    );
    this.dataService.apiDataCtfInfoGet().subscribe(
      (ctfInfo) => {
        let state = ctx.getState();
        ctx.setState({ ...state, authenticated: true, ctfInfo: ctfInfo });
      },
      (error) => {
        // Do nothing for now
      }
    );
  }

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

  @Selector()
  public static authenticated(state: AppStateModel): boolean {
    return state.authenticated;
  }

  @Selector()
  public static teamInfo(state: AppStateModel): TeamDetailsMessage | null {
    return state.teamInfo;
  }

  @Selector()
  public static ctfInfo(state: AppStateModel): CtfInfoMessage | null {
    return state.ctfInfo;
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
