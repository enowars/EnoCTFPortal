import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PageInformationComponent } from './page-information.component';

describe('PageInformationComponent', () => {
  let component: PageInformationComponent;
  let fixture: ComponentFixture<PageInformationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PageInformationComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PageInformationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
