import { Component, Input, OnInit } from '@angular/core';
import { Question } from '../../../../models/questions/base/base-question.model';
import { BinaryQuestion } from '../../../../models/questions/binary-question.model';
import { QuestionComponentBase } from '../question.component';


@Component({
  selector: 'binary-question',
  templateUrl: './binary-question.component.html',
  styleUrl: './binary-question.component.scss'
})
export class BinaryQuestionComponent extends QuestionComponentBase<boolean> {
  @Input()
  question!: BinaryQuestion;
}
