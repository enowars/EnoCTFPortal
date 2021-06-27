import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Select } from '@ngxs/store';
import {
  AccountService,
  CtfInfoMessage,
  LandingPageVulnboxStatus,
  VulnboxService,
} from 'projects/backend-api/src/lib';
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
  @Select(AppState.ctfInProgress)
  public ctfInProgress$!: Observable<CtfInfoMessage>;
  @Select(AppState.ctfCheckinOpen)
  public ctfCheckinOpen$!: Observable<boolean>;

  public vulnboxStatus: typeof LandingPageVulnboxStatus =
    LandingPageVulnboxStatus;
  constructor(
    private accountService: AccountService,
    private vulnboxService: VulnboxService,
    private _snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {}

  public checkin() {
    this.accountService.apiAccountCheckInPost().subscribe(
      (success) => {
        this._snackBar.open('You are checked in!');
      },
      (error) => {
        this._snackBar.open('Something went wrong!');
      }
    );
  }

  public start() {
    this.vulnboxService.apiVulnboxStartVulnboxPost().subscribe(
      (success) => {
        this._snackBar.open('Your machine was started!');
      },
      (error) => {
        this._snackBar.open('Something went wrong!');
      }
    );
  }

  public forceReboot() {
    this.vulnboxService.apiVulnboxResetVulnboxPost().subscribe(
      (success) => {
        this._snackBar.open('Your machine will be forcefully rebooted!');
      },
      (error) => {
        this._snackBar.open('Something went wrong!');
      }
    );
  }

  copiedToast() {
    this._snackBar.open('Copied!');
  }
}
