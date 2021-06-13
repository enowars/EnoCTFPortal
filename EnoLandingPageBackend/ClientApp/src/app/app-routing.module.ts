import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { environment } from 'src/environments/environment';
import { RootComponent } from './pages/root/root.component';

const routes: Routes = [
  {
    path: '**',
    component: RootComponent,
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
