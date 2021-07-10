import { TestBed } from '@angular/core/testing';

import { ScoreboardOverrideService } from './scoreboard-override.service';

describe('ScoreboardOverrideService', () => {
  let service: ScoreboardOverrideService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ScoreboardOverrideService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
