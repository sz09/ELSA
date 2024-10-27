import { Component, Input } from '@angular/core';
import { EssayQuestion } from '../../../../models/questions/essay-question.model';
import { QuestionComponentBase } from '../question.component';

@Component({
  selector: 'essay-question',
  templateUrl: './essay-question.component.html',
  styleUrl: './essay-question.component.scss'
})
export class EssayQuestionComponent extends QuestionComponentBase<string> {
  @Input()
  question!: EssayQuestion;

  override validate(): boolean{
    if(this.question.anyAnswer){
      return true;
    }
    
    return super.validate();
  }
  override setQuestionInfos(): void {
    this.question.correctAnswer = this.correctAnswer;
  }
}
