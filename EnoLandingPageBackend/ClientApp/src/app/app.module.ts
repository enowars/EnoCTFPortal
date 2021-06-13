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
    HttpClientModule,
    ApiModule.forRoot(apiConfigFactory),
  ],
  providers: [],
  bootstrap: [AppComponent],
})
export class AppModule {}
