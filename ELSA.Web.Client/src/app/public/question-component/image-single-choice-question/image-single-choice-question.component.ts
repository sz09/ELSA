import { Component, Input, OnInit } from '@angular/core';
import { ImageSingleChoiceQuestion } from '../../../../models/questions/image-single-choice.question.model';
import { QuestionComponentBase } from '../question.component';
import { batch } from '../../../../utilities/arrray.utilities';
import { optionPerRows } from '../../../../const/const';

@Component({
  selector: 'image-single-choice-question',
  templateUrl: './image-single-choice-question.component.html',
  styleUrl: './image-single-choice-question.component.scss'
})
export class ImageSingleChoiceQuestionComponent extends QuestionComponentBase<number> implements OnInit {
  ngOnInit(): void {
    this.batchOptions = batch(this.question.options, optionPerRows);
  }

  @Input()
  question!: ImageSingleChoiceQuestion;
  batchOptions: string[][];
 
  isSelected(index: number) {
    return index === this.answer;
  }

  select(index: number) {
    this.answer = index;
  }

  calculateIndex(iBatch: number, iOption: number){
    return iBatch * optionPerRows + iOption;
  }
}
