import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PageContactComponent } from './page-contact.component';
import { MatCardModule } from '@angular/material/card';
import { MarkdownModule } from 'ngx-markdown';
import { HttpClient, HttpClientModule } from '@angular/common/http';

@NgModule({
  declarations: [PageContactComponent],
  imports: [
    CommonModule,
    MatCardModule,
    HttpClientModule,
    MatCardModule,
    MarkdownModule.forRoot({ loader: HttpClient }),
  ],
})
export class PageContactModule {}
