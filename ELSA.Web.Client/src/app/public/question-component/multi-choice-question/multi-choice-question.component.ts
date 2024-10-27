import { Component, Input } from '@angular/core';
import { MultiChoiceQuestion } from '../../../../models/questions/multi-choice.question.model';
import { QuestionComponentBase } from '../question.component';
import { batch } from '../../../../utilities/arrray.utilities';
import { optionPerRows } from '../../../../const/const';

@Component({
  selector: 'multi-choice-question',
  templateUrl: './multi-choice-question.component.html',
  styleUrl: './multi-choice-question.component.scss'
})
export class MultiChoiceQuestionComponent extends QuestionComponentBase<number[]> {
  override collectAnswer() {
    if(!this.validate()){
      this.showWarning();
    }
    this.answer = [];
    this.question.options.forEach((_, i)=> {
      if(!!this.tempModels[i]){
        this.answer.push(i);
      }
    })
    this.onCollectedEmitter.emit(this.answer);
  }

  protected override validate(): boolean {
    if(Object.values(this.tempModels).filter(d => !!d).length === 0){
      return false;
    }

    return true;
  }
  @Input()
  question!: MultiChoiceQuestion;

  ngOnInit(): void {
    this.batchOptions = batch(this.question.options, optionPerRows);
    this.question.options.forEach((d, i) => {
      this.tempModels[i] = false;
    });
    if(this.answer && this.answer.length) {
      this.answer.forEach(i => {
        this.tempModels[i] = true;
      });
    }
  }

  batchOptions: string[][];
  tempModels: {
    [index: number]: boolean
  } = {};
 
  isSelected(index: number) {
    return this.tempModels[index];  
  }

  select(index: number) {
    this.tempModels[index] = !this.tempModels[index];
  }

  calculateIndex(iBatch: number, iOption: number){
    return iBatch * optionPerRows + iOption;
  }
}
