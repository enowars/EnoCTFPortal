import { DataSource } from '@angular/cdk/collections';
import { HttpClient } from '@angular/common/http';
import {
  Component,
  OnInit,
  ViewChild,
  AfterViewInit,
  ChangeDetectionStrategy,
  ChangeDetectorRef,
} from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSort, SortDirection } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ScoreboardInfoService } from 'projects/backend-api/src/lib';
import { ScoreboardInfo } from 'projects/backend-api/src/lib/model/scoreboardInfo';
import { ScoreboardInfoTeam } from 'projects/backend-api/src/lib/model/scoreboardInfoTeam';
import { ScoreboardService } from 'projects/backend-api/src/lib/model/scoreboardService';
import { ScoreboardTeam } from 'projects/backend-api/src/lib/model/scoreboardTeam';
import { ScoreboardTeamServiceDetails } from 'projects/backend-api/src/lib/model/scoreboardTeamServiceDetails';
import { merge, Observable, of as observableOf } from 'rxjs';
import { catchError, map, startWith, switchMap } from 'rxjs/operators';
import { DialogInfoComponent } from './dialog-info/dialog-info.component';

@Component({
  selector: 'app-page-scoreboard',
  templateUrl: './page-scoreboard.component.html',
  styleUrls: ['./page-scoreboard.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PageScoreboardComponent implements OnInit {
  public round: number = 0;
  public isCurrentRound: boolean = false;
  public displayedColumns: string[] = ['teamId'];

  // public scoreboard: ScoreboardInfo;
  public teams: ScoreboardTeam[] | undefined;
  public services: ScoreboardService[] | undefined;
  public tableData: TableData | undefined;

  public dataSource: MatTableDataSource<ScoreboardTeam> = (new MatTableDataSource([])) as any;

  public countDownConfig = {
    leftTime: 60,
    format: 'mm:ss'
  };

  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    public dialog: MatDialog,
    private _httpClient: HttpClient,
    private scoreboardInfoService: ScoreboardInfoService,
    private ref: ChangeDetectorRef
  ) {}
  
  ngOnInit(): void {
    // this.ref.detach();
  }
    ngAfterViewInit() {
      this.loadRound();

      this.scoreboardInfoService.apiScoreboardInfoScoreboardJsonGet()
      this.scoreboardInfoService.apiScoreboardInfoScoreboardroundIdJsonGet(23);
  }

  public loadRound(round: number | null = null): void {
    let suffix: any = "";
    if (round !== null) {
      suffix = Math.max(round, 0);
    }

    this._httpClient
      .get('/assets/scoreboard' + suffix + '.json')
      .subscribe((scoreboard: ScoreboardInfo) => {
        this.round = scoreboard.currentRound!;
        this.teams = scoreboard.teams!;
        this.dataSource.data = scoreboard.teams!;
        this.services = scoreboard.services!;

        let currentTime = new Date();
        let startTime = new Date(scoreboard.startTimestamp!);
        let endTime = new Date(scoreboard.endTimestamp!);
        /** @ts-ignore */
        let roundLength = endTime - startTime;
        // TODO: check if this is working
        this.isCurrentRound = (roundLength + (endTime.getTime() - currentTime.getTime()) / 1000) >= 0;

        this.countDownConfig = {
          ...this.countDownConfig,
          leftTime: (endTime.getTime() - currentTime.getTime()) / 1000,
        };

        this.teams.forEach((team) => {
          let services: any = [];

          if (team && team.serviceDetails) {
            team.serviceDetails.forEach((el) => {
              services[el.serviceId!] = el;
            });
          }
        });
        this.displayedColumns = ['teamId']
        scoreboard.services?.forEach((service) => {
          if (service.serviceId) {
            this.displayedColumns.push('service-' + service.serviceId);
          }
        });
        this.ref.markForCheck();
      });
  }

  public gotoFirstRound(): void {
    this.loadRound(0);
  }

  public gotoPreviousRound(): void {
    this.loadRound(this.round - 1);
  }

  public gotoNextRound(): void {
    // TODO: check if round exists somehow ???
    this.loadRound(this.round + 1);
  }

  public gotoCurrentRound(): void {
    this.loadRound();
  }

  public trackById(index: any, item: ScoreboardTeam) {
    return item.teamId;
  }

  openInfo(info: any) {
    this.dialog.open(DialogInfoComponent, {
      data: {
        info
      }
    });
  }
}

interface TableData {
  teamName: string;
  services: {
    [service: string]: ScoreboardService;
  };
}
