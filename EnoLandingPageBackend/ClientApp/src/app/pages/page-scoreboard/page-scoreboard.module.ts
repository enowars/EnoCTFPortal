import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PageScoreboardComponent } from './page-scoreboard.component';
import { MaterialModule } from 'src/app/material.module';
import { SharedModule } from 'src/app/shared/shared.module';
import { TableVirtualScrollModule } from 'ng-table-virtual-scroll';
import { CountdownModule } from 'ngx-countdown';
import { DialogInfoComponent } from './dialog-info/dialog-info.component';
import { PblNgridModule } from '@pebula/ngrid';
import { PblNgridDragModule } from '@pebula/ngrid/drag';
import { PblNgridTargetEventsModule } from '@pebula/ngrid/target-events';
import { PblNgridTransposeModule } from '@pebula/ngrid/transpose';
import { PblNgridBlockUiModule } from '@pebula/ngrid/block-ui';
import { PblNgridDetailRowModule } from '@pebula/ngrid/detail-row';
import { PblNgridStickyModule } from '@pebula/ngrid/sticky';
import { PblNgridStatePluginModule } from '@pebula/ngrid/state';
import { PblNgridMatSortModule } from '@pebula/ngrid-material/sort';
import { PblNgridMaterialModule } from '@pebula/ngrid-material';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { PblNgridPaginatorModule } from '@pebula/ngrid-material/paginator';
import { PblNgridCheckboxModule } from '@pebula/ngrid-material/selection-column';
import { PblNgridCellTooltipModule } from '@pebula/ngrid-material/cell-tooltip';
@NgModule({
  declarations: [PageScoreboardComponent, DialogInfoComponent],
  imports: [
    CommonModule,
    MaterialModule,
    SharedModule,
    TableVirtualScrollModule,
    CountdownModule,
    BrowserAnimationsModule,
    PblNgridModule,
    PblNgridDragModule.withDefaultTemplates(),
    PblNgridTargetEventsModule,
    PblNgridBlockUiModule,
    PblNgridTransposeModule,
    PblNgridDetailRowModule,
    PblNgridStickyModule,
    PblNgridStatePluginModule,
    PblNgridMatSortModule,
    PblNgridMaterialModule,
    PblNgridPaginatorModule,
    PblNgridCheckboxModule,
    PblNgridCellTooltipModule,
  ],
})
export class PageScoreboardModule {}
