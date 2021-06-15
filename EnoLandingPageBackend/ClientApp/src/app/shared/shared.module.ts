import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TeamComponent } from './components/team/team.component';
import { MaterialModule } from '../material.module';
import { CountryFlagComponent } from './components/country-flag/country-flag.component';
import { TeamLogoComponent } from './components/team-logo/team-logo.component';

@NgModule({
  declarations: [TeamComponent, CountryFlagComponent, TeamLogoComponent],
  imports: [CommonModule, MaterialModule],
  exports: [TeamComponent, CountryFlagComponent, TeamLogoComponent],
})
export class SharedModule {}
