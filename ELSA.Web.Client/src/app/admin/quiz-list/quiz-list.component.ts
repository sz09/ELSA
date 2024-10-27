import { Component, inject, OnInit } from '@angular/core';
import { QuizService } from '../../../services/quiz.service';
import { Direction, PAGE_DEFAULT, PAGE_SIZE_DEFAULT } from '../../../utilities/page.utilities';
import { Quiz } from '../../../models/quiz/quiz.model';
import { TranslateService } from '@ngx-translate/core';
import { DialogRef, DialogService } from '@ngneat/dialog';
import { ConfirmDialogComponent } from '../confirm-dialog/confirm-dialog.component';
import { ToastrService } from 'ngx-toastr';
import { AppInjector } from '../../app.module';
interface Data {
  id: string,
  isNew: boolean,
  title?: string
}
@Component({
  selector: 'app-quiz-list',
  templateUrl: './quiz-list.component.html',
  styleUrl: './quiz-list.component.scss'
})
export class QuizListComponent implements OnInit {

  constructor(private _quizService: QuizService, public dialog: DialogService, private _translateService: TranslateService, private _toastr: ToastrService ) { }
  ngOnInit(): void {
    this.loadData();
  }
  quizzes: Quiz[] = [];
  total: number = 0;
  payload: {
    OrderBy: string,
    Page: number,
    PageSize: number,
    Direction: Direction
  } = {
    OrderBy: "CreatedAt",
    Page: PAGE_DEFAULT,
    PageSize: PAGE_SIZE_DEFAULT, 
    Direction: Direction.Desc
  };

  loadData() {
    this._quizService.adminFetchQuizzes(this.payload.OrderBy, this.payload.Direction, this.payload.Page, this.payload.PageSize).subscribe(d => {
      this.quizzes = d.data ;
      this.total = d.total;
    })
  }

  getPage(i: number){
    return i + (this.payload.Page - 1) * this.payload.PageSize;
  }

  onPageChange(page: number){
    this.payload.Page = page;
    this.loadData();
  }

  onDelete(id: string, title: string) {
    let dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        titleKey: title
      },
      closeButton: false
    })
    dialogRef.afterClosed$.subscribe(d => {
      if (d) {
        this._quizService.adminDeleteQuiz(id).subscribe(() => {
          this.handleCompleteWorks();
        })
      }
    });
  }
  
  handleCompleteWorks() {
    this._translateService.get([
      'Common.Success'
    ]).subscribe(d => {
      this._toastr.success(d['Common.Success']);
      this.loadData();
    })
  }
}
