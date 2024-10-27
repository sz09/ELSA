import { Component, Input } from '@angular/core';

@Component({
  selector: 'score',
  templateUrl: './score.component.html',
  styleUrl: './score.component.scss'
})
export class ScoreComponent {
  @Input()
  scoredPoints: number;
  @Input()
  totalScore: number;

}
