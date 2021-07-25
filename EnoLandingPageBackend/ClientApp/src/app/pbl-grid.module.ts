import { NgModule } from '@angular/core';
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

import { PblNgridPaginatorModule } from '@pebula/ngrid-material/paginator';
import { PblNgridCheckboxModule } from '@pebula/ngrid-material/selection-column';
import { PblNgridCellTooltipModule } from '@pebula/ngrid-material/cell-tooltip';
@NgModule({
  imports: [
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
  providers: [],
  exports: [
    PblNgridModule,
    PblNgridDragModule,
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
export class PblGridModule {}
