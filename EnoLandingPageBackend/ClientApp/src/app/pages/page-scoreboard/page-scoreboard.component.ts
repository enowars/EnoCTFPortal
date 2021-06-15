import { HttpClient } from '@angular/common/http';
import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { MatSort, SortDirection } from '@angular/material/sort';
import { ScoreboardInfo } from 'projects/backend-api/src/lib/model/scoreboardInfo';
import { ScoreboardInfoTeam } from 'projects/backend-api/src/lib/model/scoreboardInfoTeam';
import { ScoreboardService } from 'projects/backend-api/src/lib/model/scoreboardService';
import { ScoreboardTeam } from 'projects/backend-api/src/lib/model/scoreboardTeam';
import { ScoreboardTeamServiceDetails } from 'projects/backend-api/src/lib/model/scoreboardTeamServiceDetails';
import { merge, Observable, of as observableOf } from 'rxjs';
import { catchError, map, startWith, switchMap } from 'rxjs/operators';

@Component({
  selector: 'app-page-scoreboard',
  templateUrl: './page-scoreboard.component.html',
  styleUrls: ['./page-scoreboard.component.scss'],
})
export class PageScoreboardComponent implements OnInit {
  public displayedColumns: string[] = ['teamName'];

  // public scoreboard: ScoreboardInfo;
  public teams: ScoreboardTeam[] | undefined;
  public services: ScoreboardService[] | undefined;
  public tableData: TableData | undefined;

  @ViewChild(MatSort) sort!: MatSort;

  constructor(private _httpClient: HttpClient) {}
  ngAfterViewInit() {}

  ngOnInit(): void {
    this._httpClient
      .get('/assets/scoreboard.json')
      .subscribe((scoreboard: ScoreboardInfo) => {
        // this.scoreboard = scoreboard;
        this.teams = scoreboard.teams!;
        this.services = scoreboard.services!;

        this.teams.forEach((team) => {
          let services: any = [];

          if (team && team.serviceDetails) {
            team.serviceDetails.forEach((el) => {
              services[el.serviceId!] = el;
            });
          }

          // team.services = services;
        });
        scoreboard.services?.forEach((service) => {
          if (service.serviceId) {
            this.displayedColumns.push('service-' + service.serviceId);
          }
        });
        // why does vs code show an error here
        //                      v
        console.log(this.teams);
      });
  }

  public trackById(index: any, item: ScoreboardTeam) {
    return item.teamId;
  }
}

interface TableData {
  teamName: string;
  services: {
    [service: string]: ScoreboardService;
  };
}
