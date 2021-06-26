import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PageInformationComponent } from './page-information.component';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { MarkdownModule } from 'ngx-markdown';
import { MatCardModule } from '@angular/material/card';

@NgModule({
  declarations: [PageInformationComponent],
  imports: [
    CommonModule,
    HttpClientModule,
    MatCardModule,
    MarkdownModule.forRoot({ loader: HttpClient }),
  ],
})
export class PageInformationModule {}
