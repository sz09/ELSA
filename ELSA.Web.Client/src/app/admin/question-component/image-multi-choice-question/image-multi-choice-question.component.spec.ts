import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ImageMultiChoiceQuestionComponent } from './image-multi-choice-question.component';

describe('ImageMultiChoiceQuestionComponent', () => {
  let component: ImageMultiChoiceQuestionComponent;
  let fixture: ComponentFixture<ImageMultiChoiceQuestionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ImageMultiChoiceQuestionComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ImageMultiChoiceQuestionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
