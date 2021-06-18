import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PageScoreboardComponent } from './page-scoreboard.component';
import { MaterialModule } from 'src/app/material.module';
import { SharedModule } from 'src/app/shared/shared.module';
import { TableVirtualScrollModule } from 'ng-table-virtual-scroll';
import { CountdownModule } from 'ngx-countdown';
import { DialogInfoComponent } from './dialog-info/dialog-info.component';

@NgModule({
  declarations: [PageScoreboardComponent, DialogInfoComponent],
  imports: [
    CommonModule,
    MaterialModule,
    SharedModule,
    TableVirtualScrollModule,
    CountdownModule,
  ],
})
export class PageScoreboardModule {}
