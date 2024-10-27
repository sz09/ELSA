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
    this.question.options.forEach((d, i) => {
      this.tempModels[i] = false;
      this.tempOptionModels[i] = d;
      this.tempFileOptionModels[i] = null;
    });
    this.calculateBatchOptions();
  }

  override setQuestionInfos(): void {
    this.question.correctAnswer = this.correctAnswer;
  }
  override collectAnswer(): number {
    this.question.newFiles = [];
    this.question.options.forEach((d, i) => {
      if(this.tempFileOptionModels[i]){
        this.question.newFiles.push({
          index: i,
          file: this.tempFileOptionModels[i]
        })
      }
    });
    
    return super.collectAnswer();
  }

  @Input()
  question!: ImageSingleChoiceQuestion;
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
    return index === this.correctAnswer;
  }

  select(index: number) {
    this.correctAnswer = index;
  }

  calculateIndex(iBatch: number, iOption: number){
    return iBatch * optionPerRows + iOption;
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
