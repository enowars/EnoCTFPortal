import { HttpClient } from '@angular/common/http';
import {
  Component,
  OnInit,
  ViewChild,
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  TrackByFunction,
} from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSort, SortDirection } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import {
  Scoreboard,
  ScoreboardInfoService,
  ScoreboardTeamServiceDetails,
} from 'projects/backend-api/src/lib';
import { ScoreboardService } from 'projects/backend-api/src/lib/model/scoreboardService';
import { ScoreboardTeam } from 'projects/backend-api/src/lib/model/scoreboardTeam';
import {
  DialogInfoComponent,
  InfoDialogData,
} from './dialog-info/dialog-info.component';
import {
  createDS,
  columnFactory,
  PblDataSource,
  PblNgridColumnSet,
  PblColumnDefinition,
} from '@pebula/ngrid';

@Component({
  selector: 'app-page-scoreboard',
  templateUrl: './page-scoreboard.component.html',
  styleUrls: ['./page-scoreboard.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { class: 'page-expand' },
})
export class PageScoreboardComponent implements OnInit {
  public columns: PblNgridColumnSet = columnFactory().build();
  public ds: PblDataSource = createDS<any>()
    .onTrigger(() => this.data)
    .create();
  public round: number = 0;
  public roundLength: number = 60;
  public isCurrentRound: boolean = false;
  public services: ScoreboardService[] | undefined;
  private data: any[] = [];

  public countDownConfig = {
    leftTime: 60,
    format: 'mm:ss',
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

    // this.scoreboardInfoService.apiScoreboardInfoScoreboardJsonGet();
    // this.scoreboardInfoService.apiScoreboardInfoScoreboardroundIdJsonGet(23);
  }

  public loadRound(round: number | null = null): void {
    let suffix: any = '';
    if (round !== null) {
      suffix = Math.max(round, 0);
    }

    this._httpClient
      .get<Scoreboard>('/api/scoreboardinfo/scoreboard' + suffix + '.json')
      .subscribe((scoreboard) => {
        let startTime = new Date(scoreboard.startTimestamp!);
        let endTime = new Date(scoreboard.endTimestamp!);
        this.round = scoreboard.currentRound;
        this.roundLength = Math.floor(
          (endTime.getTime() - startTime.getTime()) / 1000
        );
        this.services =
          scoreboard.services?.sort((a, b) => a.serviceId! - b.serviceId!) ||
          [];

        console.log(this.services);

        let currentTime = new Date();
        const timeLeft =
          (endTime.getTime() +
            this.roundLength * 1000 -
            currentTime.getTime()) /
            1000 +
          1;
        this.isCurrentRound = timeLeft >= 0;

        this.countDownConfig = {
          ...this.countDownConfig,
          leftTime: timeLeft,
        };

        this.data =
          scoreboard.teams?.map((team) => {
            let row: any = {
              team: team,
            };
            team.serviceDetails?.forEach((service) => {
              row['service-' + service.serviceId!] = service;
            });
            return row;
          }) || [];
        this.ds.refresh();
        // Only get columns on first load
        if (this.columns.table.cols.length == 0) {
          this.columns = columnFactory()
            .default({ minWidth: 200 })
            .table(
              {
                prop: 'team',
                // id: 'id',
                label: 'Team',
                minWidth: 250,
                width: '40px',
                pin: 'start',
                pIndex: true,
                wontBudge: true,
              },
              ...(scoreboard.services?.reduce((accumulator, service) => {
                let col: PblColumnDefinition = {
                  prop: 'service-' + service.serviceId,
                  label: service.serviceName,
                  minWidth: 100,
                  width: '100px',
                  reorder: true,
                  type: 'service',
                  data: service,
                };
                accumulator.push(col);
                return accumulator;
              }, [] as any[]) || [])
            )
            .build();
        }

        this.ref.markForCheck();

        if (this.isCurrentRound) {
          setTimeout(() => this.gotoCurrentRound(), (1.5 + timeLeft) * 1000);
        }
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

  public trackById: TrackByFunction<ScoreboardTeam> = (
    index: number,
    item: ScoreboardTeam
  ) => {
    return item.teamId;
  };

  openInfo(
    row: any,
    details: ScoreboardTeamServiceDetails,
    service: ScoreboardService
  ) {
    let data: InfoDialogData = {
      row: row,
      serviceDetails: details,
      service: service,
    };
    this.dialog.open(DialogInfoComponent, {
      data: data,
    });
  }
}
