import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PageScoreboardComponent } from './page-scoreboard.component';
import { MaterialModule } from 'src/app/material.module';
import { SharedModule } from 'src/app/shared/shared.module';
import { CountdownModule } from 'ngx-countdown';
import { DialogInfoComponent } from './dialog-info/dialog-info.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { PblGridModule } from 'src/app/pbl-grid.module';
@NgModule({
  declarations: [PageScoreboardComponent, DialogInfoComponent],
  imports: [
    CommonModule,
    MaterialModule,
    SharedModule,
    CountdownModule,
    BrowserAnimationsModule,
    PblGridModule,
  ],
  exports: [DialogInfoComponent],
})
export class PageScoreboardModule {}
