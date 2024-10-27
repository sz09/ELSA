import { Component, Input, OnInit } from '@angular/core';
import { MultiChoiceQuestion } from '../../../../models/questions/multi-choice.question.model';
import { QuestionComponentBase } from '../question.component';
import { batch } from '../../../../utilities/arrray.utilities';
import { optionPerRows } from '../../../../const/const';

@Component({
  selector: 'multi-choice-question',
  templateUrl: './multi-choice-question.component.html',
  styleUrl: './multi-choice-question.component.scss'
})
export class MultiChoiceQuestionComponent extends QuestionComponentBase<number[]> implements OnInit {
  override setQuestionInfos(): void {
    this.question.correctAnswers = this.correctAnswer;
  }
  override collectAnswer(): number[] {
    if(!this.validate()){
      this.showWarning();
    }
    this.correctAnswer = [];
    this.question.options.forEach((_, i)=> {
      if(!!this.tempModels[i]){
        this.correctAnswer.push(i);
      }
    });
    this.question.options = Object.values(this.tempOptionModels);
    return this.correctAnswer;
  }

  override validate(): boolean {
    if(Object.values(this.tempOptionModels).filter(d => !!d).length === 0){
      return false;
    }

    if(Object.values(this.tempModels).filter(d => !!d).length === 0){
      return false;
    }

    return true;
  }
  @Input()
  question!: MultiChoiceQuestion;

  ngOnInit(): void {
    this.question.options.forEach((d, i) => {
      this.tempModels[i] = false;
      this.tempOptionModels[i] = d;
    });
    if(this.correctAnswer && this.correctAnswer.length) {
      this.correctAnswer.forEach(i => {
        this.tempModels[i] = true;
      });
    }
    this.calculateBatchOptions();
  }

  batchOptions: string[][] = []
  tempModels: {
    [index: number]: boolean
  } = {};
  
  tempOptionModels: {
    [index: number]: string
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

  calculateBatchOptions(){
    this.batchOptions = batch(this.question.options, optionPerRows);;
  }

  onAddNewOption(){
    let newItemIndex = this.question.options.length;
    let newItemValue = '';
    this.tempModels[newItemIndex] = false;
    this.tempOptionModels[newItemIndex] = newItemValue;
    this.question.options.push(newItemValue);
    this.calculateBatchOptions();
  }
}
