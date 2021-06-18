import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AppNavigationComponent } from './app-navigation.component';
import { RouterModule } from '@angular/router';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatBadgeModule } from '@angular/material/badge';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatListModule } from '@angular/material/list';
import { CountdownModule } from 'ngx-countdown';

@NgModule({
  declarations: [AppNavigationComponent],
  imports: [
    CommonModule,
    MatSidenavModule,
    MatBadgeModule,
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatListModule,
    RouterModule,
    CountdownModule,
  ],
  exports: [AppNavigationComponent],
})
export class AppNavigationModule {}
