import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { NavigationEnd, Router } from '@angular/router';
import { Select } from '@ngxs/store';
import { untilComponentDestroyed } from '@w11k/ngx-componentdestroyed';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { MatBadgeModule } from '@angular/material/badge';
import { AppState } from 'src/app/shared/states/App.state';
import { environment } from 'src/environments/environment';
import { EnvironmentInterface } from 'src/environments/environmentInterfaces';
import { Theme } from 'src/app/shared/models/enumberables/Theme';

@Component({
  selector: 'app-navigation',
  templateUrl: './app-navigation.component.html',
  styleUrls: ['./app-navigation.component.scss'],
  host: { class: 'pb-expand' },
})
export class AppNavigationComponent implements OnInit, OnDestroy {
  public environment: EnvironmentInterface = environment;

  @Select(AppState.activeTheme)
  public themeValue$!: Observable<Theme>;

  constructor(
    private router: Router,
    public dialog: MatDialog,
    public badge: MatBadgeModule
  ) {}

  public ngOnInit() {}
  public ngOnDestroy() {}
}
