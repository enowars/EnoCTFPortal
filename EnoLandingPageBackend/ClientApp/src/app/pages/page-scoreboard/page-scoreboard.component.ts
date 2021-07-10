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
  PblColumnGroupDefinition,
} from '@pebula/ngrid';
import { ScoreboardOverrideService } from 'src/app/scoreboard-override.service';
import { TeamDetailsMessage } from 'projects/backend-api/src/lib/model/teamDetailsMessage';
import { Observable } from 'rxjs';
import { AppState } from 'src/app/shared/states/App.state';
import { Select } from '@ngxs/store';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-page-scoreboard',
  templateUrl: './page-scoreboard.component.html',
  styleUrls: ['./page-scoreboard.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { class: 'page-expand' },
})
export class PageScoreboardComponent implements OnInit {
  @Select(AppState.teamInfo)
  public teamInfo$!: Observable<TeamDetailsMessage>;
  public currentTeamId: number | undefined | null = null;

  public columns: PblNgridColumnSet = columnFactory().build();
  public ds: PblDataSource = createDS<any>()
    .onTrigger(() => this.data)
    .create();
  public round: number = -1;
  public roundLength: number = 60;
  public isCurrentRound: boolean = false;
  public isLoading: boolean = false;
  public services: ScoreboardService[] | undefined;
  private data: any[] = [];
  public reloadTimer: NodeJS.Timeout | null = null;
  public isViewingPastRounds = false;

  public countDownConfig = {
    leftTime: 60,
    format: 'mm:ss',
  };

  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    public dialog: MatDialog,
    private _httpClient: HttpClient,
    private scoreboardInfoService: ScoreboardInfoService,
    private scoreboardOverrideService: ScoreboardOverrideService,
    private ref: ChangeDetectorRef,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe((params) => {
      if (params['roundId'] && !isNaN(params['roundId'])) {
        this.round = Number(params['roundId']);
      }
    });
    this.teamInfo$.subscribe((teamInfo) => {
      this.currentTeamId = teamInfo.id;
    });
  }
  ngAfterViewInit() {
    this.loadRound(this.round);
  }

  public loadRound(round: number = -1, retryCount: number = 0): void {
    let suffix: number = -1;
    if (round >= 0) {
      suffix = Math.max(round, 0);
    }

    let previousRound = this.round;
    this.isLoading = true;
    this.ref.markForCheck();

    this.scoreboardOverrideService
      .getScoreboard(suffix)
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
            /** @ts-ignore */
            team['highlightclass'] =
              this.currentTeamId && team.teamId === this.currentTeamId
                ? 'team-cell-highlight'
                : '';
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
                id: 'team.rank',
                label: 'Rank',
                minWidth: 40,
                maxWidth: 40,
                pin: 'start',
                wontBudge: true,
              },
              {
                prop: 'team',
                label: 'Team',
                minWidth: 150,
                maxWidth: 150,
                pin: 'start',
                pIndex: true,
                wontBudge: true,
              },
              {
                prop: 'team',
                id: 'team.totalScore',
                label: 'Score',
                minWidth: 100,
                maxWidth: 100,
                pin: 'start',
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
            .headerGroup(
              {
                label: 'Team',
                columnIds: ['team.rank', 'team', 'team.totalScore'],
              },
              ...(scoreboard.services?.reduce((accumulator, service) => {
                let col = {
                  label: service.serviceName,
                  columnIds: ['service-' + service.serviceId],
                };
                accumulator.push(col);
                return accumulator;
              }, [] as any[]) || [])
            )
            .build();
        }

        this.ref.markForCheck();
        this.isLoading = false;

        if (!this.isViewingPastRounds) {
          // Only schedule a reload if the user is not vieweing past rounds
          if (this.round <= previousRound && retryCount > 0) {
            // we didn't receive a newer round, retrying again in 5 seconds
            this.isLoading = true;
            this.reloadTimer = setTimeout(
              () => this.gotoCurrentRound(retryCount - 1),
              5 * 1000
            );
          } else if (this.isCurrentRound) {
            // we received the current round, loading next round after the current round + 1.5 seconds
            this.reloadTimer = setTimeout(
              () => this.gotoCurrentRound(),
              (1.5 + timeLeft) * 1000
            );
          }
        } else {
          if (this.reloadTimer) {
            clearTimeout(this.reloadTimer);
          }
        }
      });
  }

  public gotoFirstRound(): void {
    this.isViewingPastRounds = true;
    this.loadRound(0);
  }

  public gotoPreviousRound(): void {
    this.isViewingPastRounds = true;
    this.loadRound(this.round - 1);
  }

  public gotoNextRound(): void {
    // TODO: check if round exists somehow ???
    this.isViewingPastRounds = true;
    this.loadRound(this.round + 1);
  }

  public gotoCurrentRound(retryCount: number = 2): void {
    this.isViewingPastRounds = false;
    this.loadRound(-1, retryCount);
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

  public ngOnDestroy() {
    if (this.reloadTimer) {
      clearTimeout(this.reloadTimer);
    }
  }
}
