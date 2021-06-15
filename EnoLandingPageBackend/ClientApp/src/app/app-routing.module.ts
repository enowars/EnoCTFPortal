import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { environment } from 'src/environments/environment';
import { PageAdminComponent } from './pages/page-admin/page-admin.component';
import { PageContactComponent } from './pages/page-contact/page-contact.component';
import { PageInformationComponent } from './pages/page-information/page-information.component';
import { PageProfileComponent } from './pages/page-profile/page-profile.component';
import { PageScoreboardComponent } from './pages/page-scoreboard/page-scoreboard.component';
import { PageTeamsComponent } from './pages/page-teams/page-teams.component';
import { PageRootComponent } from './pages/root/root.component';

export const APP_ROUTES = {
  scoreboard: 'scoreboard',
  teams: 'teams',
  information: 'information',
  contact: 'contact',
  profile: 'profile',
  admin: 'admin',
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
    path: APP_ROUTES.information,
    component: PageInformationComponent,
  },
  {
    path: APP_ROUTES.contact,
    component: PageContactComponent,
  },
  {
    path: APP_ROUTES.profile,
    component: PageProfileComponent,
  },
  {
    path: APP_ROUTES.admin,
    component: PageAdminComponent,
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
