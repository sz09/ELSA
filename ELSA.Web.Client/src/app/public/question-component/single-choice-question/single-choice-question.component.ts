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
export class SingleChoiceQuestionComponent extends QuestionComponentBase<number> implements OnInit {
  ngOnInit(): void {
    this.question.options.forEach((d, i) => {
      this.tempModels[i] = false;
    });
  }
  override collectAnswer() {
    if(!this.validate()){
      this.showWarning();
      return;
    }
    this.question.options.forEach((_, i)=> {
      if(!!this.tempModels[i]){
        this.answer = i;
        return;
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
  get batchOptions() {
    return batch(this.question.options ?? [], optionPerRows)
  } 
  @Input()
  question!: SingleChoiceQuestion;

  tempModels: {
    [index: number]: boolean
  } = {};
  
  calculateIndex(iBatch: number, iOption: number){
    return iBatch * optionPerRows + iOption;
  }

  select(index: number) {
    for(let i = 0; i < this.question.options.length; i ++){
      this.tempModels[i]  = false;
    } 
    this.tempModels[index] = true;
  }
}
