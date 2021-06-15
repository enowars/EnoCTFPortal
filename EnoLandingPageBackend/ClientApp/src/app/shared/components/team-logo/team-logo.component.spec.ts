import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TeamLogoComponent } from './team-logo.component';

describe('TeamLogoComponent', () => {
  let component: TeamLogoComponent;
  let fixture: ComponentFixture<TeamLogoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TeamLogoComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TeamLogoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
