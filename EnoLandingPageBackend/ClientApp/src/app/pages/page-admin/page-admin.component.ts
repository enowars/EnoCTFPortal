import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import {
  AdminService,
  DataService,
  TeamMessage,
  TeamsMessage,
} from 'projects/backend-api/src/lib';

@Component({
  selector: 'app-page-admin',
  templateUrl: './page-admin.component.html',
  styleUrls: ['./page-admin.component.scss'],
})
export class PageAdminComponent implements OnInit {
  public secret: string = '';
  public selectedTeam: number | undefined;
  public teams: TeamMessage[] | null = null;

  constructor(
    private adminService: AdminService,
    private _dataService: DataService,
    private _snackBar: MatSnackBar
  ) {}
  ngOnInit(): void {
    this._dataService.apiDataTeamsGet().subscribe((info) => {
      this.teams = [...info.registeredTeams!, ...info.confirmedTeams!];
    });
  }

  restartMachine() {
    this.adminService
      .apiAdminBootVmGet(this.secret, this.selectedTeam)
      .subscribe(
        (success) => {
          this._snackBar.open('Rebooted!');
        },
        (error) => {
          this._snackBar.open('Something went wrong!');
        }
      );
  }
}
