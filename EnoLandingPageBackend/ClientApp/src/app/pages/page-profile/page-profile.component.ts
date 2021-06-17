import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Select } from '@ngxs/store';
import { CtfInfoMessage } from 'projects/backend-api/src/lib';
import { TeamDetailsMessage } from 'projects/backend-api/src/lib/model/teamDetailsMessage';
import { Observable } from 'rxjs';
import { AppState } from 'src/app/shared/states/App.state';

@Component({
  selector: 'app-page-profile',
  templateUrl: './page-profile.component.html',
  styleUrls: ['./page-profile.component.scss'],
})
export class PageProfileComponent implements OnInit {
  @Select(AppState.teamInfo)
  public teamInfo$!: Observable<TeamDetailsMessage>;
  @Select(AppState.ctfInfo)
  public ctfInfo$!: Observable<CtfInfoMessage>;
  constructor(private _snackBar: MatSnackBar) {}

  ngOnInit(): void {}

  copiedToast() {
    this._snackBar.open('Copied!');
  }
}
