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

@Component({
  selector: 'app-navigation',
  templateUrl: './app-navigation.component.html',
  styleUrls: ['./app-navigation.component.scss'],
  host: { class: 'pb-expand' },
})
export class AppNavigationComponent
  extends OnDestroyMixin
  implements OnInit, OnDestroy
{
  public environment: EnvironmentInterface = environment;
  public routes: typeof APP_ROUTES = APP_ROUTES;
  public themeValue: Theme | null = null;
  @Select(AppState.authenticated)
  public authenticated$!: Observable<boolean>;
  @Select(AppState.teamInfo)
  public info$!: Observable<TeamDetailsMessage>;

  constructor(private store: Store) {
    super();
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
  public ngOnInit() {
    this.store
      .select(AppState)
      .pipe(untilComponentDestroyed(this))
      .subscribe((state: AppStateModel) => {
        this.themeValue = state.activeTheme;
      });
  }
  public ngOnDestroy() {}
}
