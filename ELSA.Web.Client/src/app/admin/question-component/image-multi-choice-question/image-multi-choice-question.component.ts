import { Component, Input, OnInit } from '@angular/core';
import { ImageMultiChoiceQuestion } from '../../../../models/questions/image-multi-choice.question.model';
import { QuestionComponentBase } from '../question.component';
import { batch } from '../../../../utilities/arrray.utilities';
import { optionPerRows } from '../../../../const/const';

@Component({
  selector: 'image-multi-choice-question',
  templateUrl: './image-multi-choice-question.component.html',
  styleUrl: './image-multi-choice-question.component.scss'
})
export class ImageMultiChoiceQuestionComponent extends QuestionComponentBase<number[]>  implements OnInit {
  @Input()
  question!: ImageMultiChoiceQuestion;
  override collectAnswer(): number[] {
    if(!this.validate()){
      this.showWarning();
    }
    this.correctAnswer = [];
    this.question.options.forEach((_, i)=> {
      if(!!this.tempModels[i]){
        this.correctAnswer.push(i);
      }
    })
    
    this.question.newFiles = [];
    this.question.options.forEach((d, i) => {
      if(this.tempFileOptionModels[i]){
        this.question.newFiles.push({
          index: i,
          file: this.tempFileOptionModels[i]
        })
      }
    });

    return this.correctAnswer;
  }

  override setQuestionInfos(): void {
    this.question.correctAnswers = this.correctAnswer;
  }

  override validate(): boolean {
    if(Object.values(this.tempOptionModels).filter(d => !!d).length === 0 && 
       Object.values(this.tempOptionModels).length === 0){
      return false;
    }
    if(Object.values(this.tempModels).filter(d => !!d).length === 0){
      return false;
    }

    return true;
  }

  ngOnInit(): void {
    this.question.options.forEach((d, i) => {
      this.tempModels[i] = false;
      this.tempOptionModels[i] = d;
      this.tempFileOptionModels[i] = null;
    });
    if(this.correctAnswer && this.correctAnswer.length) {
      this.correctAnswer.forEach(i => {
        this.tempModels[i] = true;
      });
    }
    this.calculateBatchOptions();
  }

  batchOptions: string[][];
  tempModels: {
    [index: number]: boolean
  } = {};
  
  tempOptionModels: {
    [index: number]: string
  } = {};
 
  tempFileOptionModels: {
    [index: number]: File
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

  
  onChangeImage(index: number, event: any){
    const file =  event.target.files[0];
    const reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onload = () => {
      if(typeof reader.result === 'string'){
        this.tempOptionModels[index] = reader.result;
      }
    };
    this.tempFileOptionModels[index] = file;
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
