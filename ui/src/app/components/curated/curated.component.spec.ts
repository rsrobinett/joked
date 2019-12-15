import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CuratedComponent } from './curated.component';

describe('CuratedJokeComponent', () => {
  let component: CuratedComponent;
  let fixture: ComponentFixture<CuratedComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CuratedComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CuratedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
