import { Component, OnInit } from '@angular/core';
import { QuizService } from '../../../services/quiz.service';
import { Direction, ORDER_BY_DEFAULT, PAGE_DEFAULT, PAGE_SIZE_DEFAULT } from '../../../utilities/page.utilities';
import { Quiz } from '../../../models/quiz/quiz.model';
import { itemsPerRows } from '../../../const/const';
import { batch } from '../../../utilities/arrray.utilities';

@Component({
  selector: 'app-quiz-list',
  templateUrl: './quiz-list.component.html',
  styleUrl: './quiz-list.component.scss'
})
export class QuizListComponent implements OnInit {

  constructor(private _quizService: QuizService) { }
  payload: {
    OrderBy: string,
    Page: number,
    PageSize: number,
    Direction: Direction
  } = {
    OrderBy: "QuestionLength",
    Page: PAGE_DEFAULT,
    PageSize: PAGE_SIZE_DEFAULT, 
    Direction: Direction.Desc
  };

  quizzes: Quiz[] = [];
  get quizRows(): Quiz[][]  {
    return batch(this.quizzes, itemsPerRows);
  }
  total: number = 0;
  ngOnInit(): void {
    this.loadData();
  }
  loadData() {
    this._quizService.fetchQuizzes(this.payload.OrderBy, this.payload.Direction, this.payload.Page, this.payload.PageSize).subscribe(d => {
      this.quizzes = this.quizzes.concat(d.data);
      this.total = d.total;
    })
  }
  loadMore(){
    this.payload.Page ++;
    this.loadData();
  }
}
