import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PageScoreboardComponent } from './page-scoreboard.component';
import { MaterialModule } from 'src/app/material.module';

@NgModule({
  declarations: [PageScoreboardComponent],
  imports: [CommonModule, MaterialModule],
})
export class PageScoreboardModule {}
