import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import {
  ApiModule,
  Configuration,
  ConfigurationParameters,
} from 'projects/backend-api/src/lib';
import { environment } from 'src/environments/environment';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HttpClientModule } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppNavigationModule } from 'src/app/modules/app-navigation/app-navigation.module';
import { MaterialModule } from './material.module';
import { NgxsModule } from '@ngxs/store';
import { AppState } from './shared/states/App.state';
import { ThemeService } from './services/theme.service';
import { PageRootModule } from './pages/root/root.module';
import { PageTeamsModule } from './pages/page-teams/page-teams.module';
import { PageScoreboardModule } from './pages/page-scoreboard/page-scoreboard.module';
import { OAuthModule } from 'angular-oauth2-oidc';
export function apiConfigFactory(): Configuration {
  const params: ConfigurationParameters = {
    basePath: environment.backendBaseUrl,
    // set configuration parameters here.
  };
  return new Configuration(params);
}

@NgModule({
  declarations: [AppComponent],
  imports: [
    BrowserModule,
    AppRoutingModule,
    AppNavigationModule,
    MaterialModule,
    HttpClientModule,
    ApiModule.forRoot(apiConfigFactory),
    NgxsModule.forRoot([AppState], {
      developmentMode: !environment.production,
    }),
    OAuthModule.forRoot(),
    PageRootModule,
    PageTeamsModule,
    PageScoreboardModule,
    BrowserAnimationsModule,
  ],
  providers: [ThemeService],
  bootstrap: [AppComponent],
})
export class AppModule {}
