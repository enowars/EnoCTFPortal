import { Component, OnInit } from '@angular/core';
import { DataService, TeamsMessage } from 'projects/backend-api/src/lib';

@Component({
  selector: 'app-page-teams',
  templateUrl: './page-teams.component.html',
  styleUrls: ['./page-teams.component.scss'],
})
export class PageTeamsComponent implements OnInit {
  public teams: TeamsMessage | null = null;
  constructor(private _dataService: DataService) {}

  ngOnInit(): void {
    this._dataService.apiDataTeamsGet().subscribe((info) => {
      this.teams = info;
    });
  }
}
