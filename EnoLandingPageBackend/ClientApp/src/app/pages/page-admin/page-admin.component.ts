import { Component, OnInit } from '@angular/core';
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
    private _dataService: DataService
  ) {}
  ngOnInit(): void {
    this._dataService.apiDataTeamsGet().subscribe((info) => {
      this.teams = [...info.registeredTeams!, ...info.confirmedTeams!];
    });
  }

  restartMachine() {
    this.adminService.apiAdminBootVmGet(this.secret, this.selectedTeam).subscribe(
      (success) => {},
      (error) => {}
    );
  }
}
