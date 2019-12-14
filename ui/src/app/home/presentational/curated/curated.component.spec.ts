import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CuratedJokeComponent } from './curated.component';

describe('CuratedJokeComponent', () => {
  let component: CuratedJokeComponent;
  let fixture: ComponentFixture<CuratedJokeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CuratedJokeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CuratedJokeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
