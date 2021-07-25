import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PageProfileComponent } from './page-profile.component';
import { MaterialModule } from 'src/app/material.module';

@NgModule({
  declarations: [PageProfileComponent],
  imports: [CommonModule, MaterialModule],
})
export class PageProfileModule {}
