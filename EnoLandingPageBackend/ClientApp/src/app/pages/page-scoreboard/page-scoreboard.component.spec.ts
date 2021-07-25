import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PageScoreboardComponent } from './page-scoreboard.component';

describe('PageScoreboardComponent', () => {
  let component: PageScoreboardComponent;
  let fixture: ComponentFixture<PageScoreboardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PageScoreboardComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PageScoreboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
