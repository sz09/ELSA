import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AnswerState, QuestionState } from '../../../models/answers/answer-state.enum';

@Component({
  selector: 'progression',
  templateUrl: './progression.component.html',
  styleUrl: './progression.component.scss'
})
export class ProgressionComponent implements OnInit {
  get completed(): number {
    return this.questionStates.filter(d => d.state !== AnswerState.NotAnswered).length;
  }
  @Input()
  total: number;
  @Input()
  questionStates: QuestionState[];
  
  @Output()
  reelectQuestionEmitter: EventEmitter<number> = new EventEmitter<number>();
  ngOnInit(): void {
    this.withPercent = 100 / this.questionStates.length;
  }

  withPercent: number = 0;
  answerState = AnswerState;
}
