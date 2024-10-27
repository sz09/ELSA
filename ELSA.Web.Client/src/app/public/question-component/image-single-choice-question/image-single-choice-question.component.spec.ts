import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ImageSingleChoiceQuestionComponent } from './image-single-choice-question.component';

describe('ImageSingleChoiceQuestionComponent', () => {
  let component: ImageSingleChoiceQuestionComponent;
  let fixture: ComponentFixture<ImageSingleChoiceQuestionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ImageSingleChoiceQuestionComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ImageSingleChoiceQuestionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
