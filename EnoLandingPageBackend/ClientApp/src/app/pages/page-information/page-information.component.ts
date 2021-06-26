import { Component, OnInit } from '@angular/core';
import { Select } from '@ngxs/store';
import { CtfInfoMessage } from 'projects/backend-api/src/lib';
import { Observable } from 'rxjs';
import { AppState } from 'src/app/shared/states/App.state';

@Component({
  selector: 'app-page-information',
  templateUrl: './page-information.component.html',
  styleUrls: ['./page-information.component.scss'],
  host: { class: 'page-expand' },
})
export class PageInformationComponent implements OnInit {
  @Select(AppState.ctfInfo)
  public ctfInfo$!: Observable<CtfInfoMessage>;
  constructor() {}

  ngOnInit(): void {}
}
