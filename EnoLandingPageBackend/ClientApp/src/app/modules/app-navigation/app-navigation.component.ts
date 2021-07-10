import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { NavigationEnd, Router } from '@angular/router';
import { Select, Store } from '@ngxs/store';
import {
  OnDestroyMixin,
  untilComponentDestroyed,
} from '@w11k/ngx-componentdestroyed';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { MatBadgeModule } from '@angular/material/badge';
import {
  AppState,
  AppStateModel,
  SetTheme,
} from 'src/app/shared/states/App.state';
import { environment } from 'src/environments/environment';
import { EnvironmentInterface } from 'src/environments/environmentInterfaces';
import { Theme } from 'src/app/shared/models/enumberables/theme';
import { APP_ROUTES } from 'src/app/app-routing.module';
import { coerceStringArray } from '@angular/cdk/coercion';
import { TeamDetailsMessage } from 'projects/backend-api/src/lib/model/teamDetailsMessage';
import { CtfInfoMessage } from 'projects/backend-api/src/lib';
import {
  runtimeEnvironment,
  RuntimeEnvironmentInterface,
} from 'src/environments/runtime-environment';
import { CountdownEvent } from 'ngx-countdown';

@Component({
  selector: 'app-navigation',
  templateUrl: './app-navigation.component.html',
  styleUrls: ['./app-navigation.component.scss'],
  host: { class: 'page-expand' },
})
export class AppNavigationComponent
  extends OnDestroyMixin
  implements OnInit, OnDestroy
{
  public environment: EnvironmentInterface = environment;
  public staticHosting: boolean = runtimeEnvironment.staticHosting == 'true';
  public routes: typeof APP_ROUTES = APP_ROUTES;
  public themeValue: Theme | null = null;
  @Select(AppState.authenticated)
  public authenticated$!: Observable<boolean>;
  @Select(AppState.teamInfo)
  public info$!: Observable<TeamDetailsMessage>;
  @Select(AppState.ctfInfo)
  public ctfInfo$!: Observable<CtfInfoMessage>;
  @Select(AppState.ctfInProgress)
  public ctfInProgress$!: Observable<boolean>;

  @Select(AppState.ctfCheckinOpen)
  public ctfCheckinOpen$!: Observable<boolean>;

  @Select(AppState.ctfRegistrationOpen)
  public ctfRegistrationOpen$!: Observable<boolean>;
  @Select(AppState.ctfIsOver)
  public ctfIsOver$!: Observable<boolean>;

  public countDownConfig = {
    leftTime: 60,
    format: 'HH:mm:ss',
  };

  constructor(private store: Store) {
    super();
  }

  public countdownend(event: CountdownEvent) {
    if (event.action == 'done') {
      this.refreshTimer(this.store.selectSnapshot(AppState));
    }
  }
  public toggleTheme() {
    if (this.themeValue == Theme.default_dark) {
      this.store.dispatch(new SetTheme(Theme.default_light));
    } else {
      this.store.dispatch(new SetTheme(Theme.default_dark));
    }
  }

  public getThemeIcon() {
    if (this.themeValue == Theme.default_dark) {
      return 'light_mode';
    } else {
      return 'dark_mode';
    }
  }

  public refreshTimer(state: AppStateModel) {
    if (state.ctfInfo != null) {
      if (AppState.ctfInProgress(state)) {
        // CTF is in progress
        this.countDownConfig = {
          ...this.countDownConfig,
          leftTime:
            (Date.parse(state.ctfInfo.ctfEndTime) - new Date().getTime()) /
            1000,
        };
      } else if (AppState.ctfCheckinOpen(state)) {
        this.countDownConfig = {
          ...this.countDownConfig,
          leftTime:
            (Date.parse(state.ctfInfo.checkInEndTime) - new Date().getTime()) /
            1000,
        };
      } else if (AppState.ctfRegistrationOpen(state)) {
        this.countDownConfig = {
          ...this.countDownConfig,
          leftTime:
            (Date.parse(state.ctfInfo.registrationCloseTime) -
              new Date().getTime()) /
            1000,
          format: 'dd:HH:mm:ss',
        };
      } else {
        let leftTime =
          (Date.parse(state.ctfInfo.ctfStartTime) - new Date().getTime()) /
          1000;
        this.countDownConfig = {
          ...this.countDownConfig,
          leftTime: leftTime,
          // If there are more than 24 hours display days
          format: leftTime > 86400 ? 'dd:HH:mm:ss' : 'HH:mm:ss',
        };
      }
    }
  }

  public get currentUrl(): string {
    return window.location.href;
  }
  public ngOnInit() {
    this.store
      .select(AppState)
      .pipe(untilComponentDestroyed(this))
      .subscribe((state: AppStateModel) => {
        this.themeValue = state.activeTheme;
        this.refreshTimer(state);
      });
  }
  public ngOnDestroy() {}
}
