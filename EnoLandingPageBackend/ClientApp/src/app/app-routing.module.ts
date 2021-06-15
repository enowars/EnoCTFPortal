import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { environment } from 'src/environments/environment';
import { PageScoreboardComponent } from './pages/page-scoreboard/page-scoreboard.component';
import { PageTeamsComponent } from './pages/page-teams/page-teams.component';
import { PageRootComponent } from './pages/root/root.component';

export const APP_ROUTES = {
  scoreboard: 'scoreboard',
  teams: 'teams',
};

const routes: Routes = [
  {
    path: APP_ROUTES.scoreboard,
    component: PageScoreboardComponent,
  },
  {
    path: APP_ROUTES.teams,
    component: PageTeamsComponent,
  },
  {
    path: '**',
    component: PageRootComponent,
  },
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes, {
      enableTracing: environment.routeTracing,
    }),
  ],
  exports: [RouterModule],
})
export class AppRoutingModule {}
