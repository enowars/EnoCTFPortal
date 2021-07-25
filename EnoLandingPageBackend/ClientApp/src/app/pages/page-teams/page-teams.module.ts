import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PageTeamsComponent } from './page-teams.component';
import { MaterialModule } from 'src/app/material.module';
import { SharedModule } from 'src/app/shared/shared.module';

@NgModule({
  declarations: [PageTeamsComponent],
  imports: [CommonModule, MaterialModule, SharedModule],
})
export class PageTeamsModule {}
