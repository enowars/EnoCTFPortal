import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../material.module';
import { CountryFlagComponent } from './components/country-flag/country-flag.component';

@NgModule({
  declarations: [CountryFlagComponent],
  imports: [CommonModule, MaterialModule],
  exports: [CountryFlagComponent],
})
export class SharedModule {}
