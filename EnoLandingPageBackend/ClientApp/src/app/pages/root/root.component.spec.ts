import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PageRootComponent } from './root.component';

describe('RootComponent', () => {
  let component: PageRootComponent;
  let fixture: ComponentFixture<PageRootComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [PageRootComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PageRootComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
