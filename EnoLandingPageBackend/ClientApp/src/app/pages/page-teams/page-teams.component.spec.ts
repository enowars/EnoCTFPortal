import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PageTeamsComponent } from './page-teams.component';

describe('PageTeamsComponent', () => {
  let component: PageTeamsComponent;
  let fixture: ComponentFixture<PageTeamsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PageTeamsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PageTeamsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
