import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PageScoreboardComponent } from './page-scoreboard.component';
import { MaterialModule } from 'src/app/material.module';
import { SharedModule } from 'src/app/shared/shared.module';
import { TableVirtualScrollModule } from 'ng-table-virtual-scroll';
@NgModule({
  declarations: [PageScoreboardComponent],
  imports: [
    CommonModule,
    MaterialModule,
    SharedModule,
    TableVirtualScrollModule,
  ],
})
export class PageScoreboardModule {}
