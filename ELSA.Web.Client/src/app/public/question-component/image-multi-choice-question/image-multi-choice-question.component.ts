import { Component, Input } from '@angular/core';
import { ImageMultiChoiceQuestion } from '../../../../models/questions/image-multi-choice.question.model';
import { QuestionComponentBase } from '../question.component';
import { batch } from '../../../../utilities/arrray.utilities';
import { optionPerRows } from '../../../../const/const';

@Component({
  selector: 'image-multi-choice-question',
  templateUrl: './image-multi-choice-question.component.html',
  styleUrl: './image-multi-choice-question.component.scss'
})
export class ImageMultiChoiceQuestionComponent extends QuestionComponentBase<number[]> {
  @Input()
  question!: ImageMultiChoiceQuestion;
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
