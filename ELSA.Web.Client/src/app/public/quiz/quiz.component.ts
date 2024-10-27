import { Component, Input } from '@angular/core';
import { Quiz } from '../../../models/quiz/quiz.model';

@Component({
  selector: 'quiz',
  templateUrl: './quiz.component.html',
  styleUrl: './quiz.component.scss'
})
export class QuizComponent {
  @Input()
  quiz: Quiz;
}
