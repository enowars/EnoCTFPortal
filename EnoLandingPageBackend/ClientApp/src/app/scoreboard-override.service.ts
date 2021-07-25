import { Injectable } from '@angular/core';
import {
  Scoreboard,
  ScoreboardTeam,
  ScoreboardInfoService,
  ScoreboardService,
  ScoreboardFirstBlood,
} from 'projects/backend-api/src/lib';
import { first, map } from 'rxjs/operators';

/**
 * This service is just here to override some quirks of the scoreboard.json that should be fixed upstream.
 */
@Injectable({
  providedIn: 'root',
})
export class ScoreboardOverrideService {
  constructor(private scoreboardInfoService: ScoreboardInfoService) {}

  private overrideScoreboard(scoreboard: Scoreboard): OverrideScorebaord {
    return {
      ...scoreboard,
      teams: scoreboard.teams.map((team, index) => {
        return {
          ...team,
          rank: index + 1,
        };
      }),
      services: scoreboard.services.map((service) => {
        return {
          ...service,
          flagstores: [...Array(service.flagVariants).keys()].map((v, i) => {
            let firstBlood = service.firstBloods.find(
              (firtBlood) => firtBlood.flagVariantId == i
            );
            return {
              variantId: i,
              firstBlood: firstBlood,
            };
          }),
        };
      }),
    };
  }

  public getScoreboard(round: number = -1) {
    if (round >= 0) {
      return this.scoreboardInfoService
        .apiScoreboardInfoScoreboardroundIdJsonGet(round)
        .pipe(map((x) => this.overrideScoreboard(x)));
    }
    return this.scoreboardInfoService
      .apiScoreboardInfoScoreboardJsonGet()
      .pipe(map((x) => this.overrideScoreboard(x)));
  }
}

export interface OverrideScorebaord extends Scoreboard {
  currentRound: number;
  startTimestamp?: string | null;
  endTimestamp?: string | null;
  dnsSuffix?: string | null;
  services: Array<OverrideScoreboardService>;
  teams: Array<OverrideScoreboardTeam>;
}
export interface OverrideScoreboardTeam extends ScoreboardTeam {
  rank: number;
}

export interface OverrideScoreboardService extends ScoreboardService {
  flagstores: {
    variantId: number;
    firstBlood: undefined | ScoreboardFirstBlood;
  }[];
}
