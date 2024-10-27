import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BinaryQuestionComponent } from './binary-question.component';

describe('BinaryQuestionComponent', () => {
  let component: BinaryQuestionComponent;
  let fixture: ComponentFixture<BinaryQuestionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [BinaryQuestionComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BinaryQuestionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
