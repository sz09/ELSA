import { Component, Input, OnInit } from '@angular/core';
import { SingleChoiceQuestion } from '../../../../models/questions/single-choice.question.model';
import { QuestionComponentBase } from '../question.component';
import { batch } from '../../../../utilities/arrray.utilities';
import { optionPerRows } from '../../../../const/const';

@Component({
  selector: 'single-choice-question',
  templateUrl: './single-choice-question.component.html',
  styleUrl: './single-choice-question.component.scss'
})
export class SingleChoiceQuestionComponent extends QuestionComponentBase<number> {
  get batchOptions() {
    return batch(this.question.options ?? [], optionPerRows)
  } 
  @Input()
  question!: SingleChoiceQuestion;

  
  calculateIndex(iBatch: number, iOption: number){
    return iBatch * optionPerRows + iOption;
  }
}
