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
      this.tempOptionModels[i] = d;
    });
    this.calculateBatchOptions();
  }
  override setQuestionInfos(): void {
    this.question.correctAnswer = this.correctAnswer;
  }
  override validate(): boolean {
    this.question.options = Object.values(this.tempOptionModels);
    if(!this.question.options || !this.question.options.length){
      return false;
    }
    
    return super.validate();
  }
  @Input()
  question!: SingleChoiceQuestion;

  
  batchOptions: string[][] = []
  tempOptionModels: {
    [index: number]: string
  } = {};
 
  calculateIndex(iBatch: number, iOption: number){
    return iBatch * optionPerRows + iOption;
  }
  

  calculateBatchOptions(){
    this.batchOptions = batch(this.question.options, optionPerRows);;
  }
  onAddNewOption(){
    let newItemIndex = this.question.options.length;
    let newItemValue = '';
    this.tempOptionModels[newItemIndex] = newItemValue;
    this.question.options.push(newItemValue);
    this.calculateBatchOptions();
  }
}
