import { Component, OnInit, QueryList, ViewChild, ViewChildren } from '@angular/core';
import { Quiz } from '../../../models/quiz/quiz.model';
import { QuizService } from '../../../services/quiz.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Question } from '../../../models/questions/base/base-question.model';
import { QuestionService } from '../../../services/question.service';
import { QuestionType } from '../../../enums/question.type';
import { SingleChoiceQuestion } from '../../../models/questions/single-choice.question.model';
import { MultiChoiceQuestion } from '../../../models/questions/multi-choice.question.model';
import { ImageSingleChoiceQuestion } from '../../../models/questions/image-single-choice.question.model';
import { ImageMultiChoiceQuestion } from '../../../models/questions/image-multi-choice.question.model';
import { BinaryQuestion } from '../../../models/questions/binary-question.model';
import { EssayQuestion } from '../../../models/questions/essay-question.model';
import { QuestionComponentBase } from '../question-component/question.component';
import { TranslateService } from '@ngx-translate/core';
import { NgForm } from '@angular/forms';
import { catchError, delay, from, map, Observable, of, Subscription } from 'rxjs';
import { concatMap, retry } from 'rxjs/operators';
import { markAllControlDirty } from '../../../utilities/form.utilites';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-quiz',
  templateUrl: './quiz.component.html',
  styleUrl: './quiz.component.scss'
})
export class QuizComponent implements OnInit {
  constructor(private _quizService: QuizService, private _questionService: QuestionService, 
    private _route: ActivatedRoute, private _translate: TranslateService,
    private _toastr: ToastrService, private _router: Router) {

  }

  ngOnInit(): void {
    this.loadQuestionTypes();
    this.initData();
  }

  @ViewChildren('questionComponent')
  questionComponents!: QueryList<QuestionComponentBase<any>>

  @ViewChild('f')
  private form: NgForm;
  isCreateNew: boolean = false;
  quiz: Quiz = new Quiz();
  deleteQuestionIds: string[] = [];
  questions: Question[] = [];
  questionTypes: {
    id: QuestionType,
    label: string
  }[] = [
    ];

  loadQuestionTypes() {
    this._translate.get([
      "Admin.QuestionType.Binary",
      "Admin.QuestionType.Essay",
      "Admin.QuestionType.SingleChoice",
      "Admin.QuestionType.MultiChoice",
      "Admin.QuestionType.ImageSingleChoice",
      "Admin.QuestionType.ImageMultiChoice",
    ]).subscribe(d => {
      this.questionTypes.push({
        id: QuestionType.Binary,
        label: d["Admin.QuestionType.Binary"]
      });
      this.questionTypes.push({
        id: QuestionType.Essay,
        label: d["Admin.QuestionType.Essay"]
      });
      this.questionTypes.push({
        id: QuestionType.SingleChoice,
        label: d["Admin.QuestionType.SingleChoice"]
      });
      this.questionTypes.push({
        id: QuestionType.MultiChoice,
        label: d["Admin.QuestionType.MultiChoice"]
      });
      this.questionTypes.push({
        id: QuestionType.ImageSingleChoice,
        label: d["Admin.QuestionType.ImageSingleChoice"]
      });
      this.questionTypes.push({
        id: QuestionType.ImageMultiChoice,
        label: d["Admin.QuestionType.ImageMultiChoice"]
      });
    })
  }
  initData() {
    this._route.params.subscribe(d => {
      this.isCreateNew = Object.keys(d).indexOf('id') === -1;
      if(!this.isCreateNew){
        let quizId = d['id'];
        if (quizId) {
          this._quizService.adminLoadQuiz(quizId).subscribe(d => {
            this.quiz = d;
          })

          this._questionService.adminLoadQuestionFromQuiz(quizId).subscribe(d => {
            this.questions = d;
          })
        }
      }
    });
  }

  addNewQuestion() {
    let id = new Date().getTime().toString();
    this.questions.push(new EssayQuestion(id));
    setTimeout(() => {
      let elementId = `question-boundary-${id}`;
      let el = document.getElementById(elementId);
      el.scrollIntoView();
    })
  }

  onChangeQuestionType(i: number, questionType: QuestionType) {
    let tempId = new Date().getTime().toString();
    switch (questionType) {
      case QuestionType.Binary:
        this.questions[i] = new BinaryQuestion(tempId)
        break;
      case QuestionType.Essay:
        this.questions[i] = new EssayQuestion(tempId)
        break;
      case QuestionType.SingleChoice:
        this.questions[i] = new SingleChoiceQuestion(tempId)
        break;
      case QuestionType.MultiChoice:
        this.questions[i] = new MultiChoiceQuestion(tempId)
        break;
      case QuestionType.ImageSingleChoice:
        this.questions[i] = new ImageSingleChoiceQuestion(tempId)
        break;
      case QuestionType.ImageMultiChoice:
        this.questions[i] = new ImageMultiChoiceQuestion(tempId)
        break;
    }
  }

  saveAll() {
    markAllControlDirty(this.form);
    if (!this.quiz.subject) {
      return;
    } 

    let states = this.questionComponents.map(d => {
      d.collectAnswer();
      return d.validate();
    });
    if(states.indexOf(false) > -1){
      this.showError();
      return;
    }
    this.addOrUpdateQuiz().subscribe({
      next: e => {
        this.isCreateNew = false;
        from(this.questions)
          .pipe(concatMap((d, index) => {
            return this.saveQuestionAt(index);
          })
        )
        .subscribe({
          next: question => console.log('Updated question:', question),
          complete: () => console.log('All questions have been updated.')
        }).add(new Subscription(() => {
          this.showSuccess();
          setTimeout(() => this._router.navigateByUrl(`/admin/quiz/${this.quiz.id}`))
        }))
      }
    })
  }
  saveQuestionByIndex(index: number){
    let state = this.questionComponents.get(index).validate();
    if(!state){
      this.showError();
    }

    this.saveQuestionAt(index).subscribe(d => {
      this.showSuccess();
    })
  }
  saveQuestionAt(index: number) {
    this.questionComponents.get(index).collectAnswer();
    this.questionComponents.get(index).setQuestionInfos();
    return this.addOrUpdateQuestion(index);
  }

  addOrUpdateQuiz(): Observable<Quiz>{
    if(this.isCreateNew){
      return this._quizService.adminCreateQuiz(this.quiz).pipe(
        map(quiz => {
          this.quiz.id = quiz.id;
          return quiz;
        }));
    }

    return this._quizService.adminUpdateQuiz(this.quiz);
  }

  addOrUpdateQuestion(index: number): Observable<Question>{
    let fileToPush: {index: number, file: File}[]  = [];
    if([QuestionType.ImageMultiChoice, QuestionType.ImageSingleChoice].indexOf(this.questions[index].type) > -1){
      let question = {...this.questions[index]} as any;
      question.newFiles?.forEach((d: any) => {
        fileToPush.push({
          file: d.file,
          index: d.index
        })
      })
    }
    
    if(this.questions[index].isNewQuestion){
     return this._questionService.adminCreateQuestion(this.questions[index], fileToPush).pipe(
      map(savedQuestion => {
        this.questions[index].isNewQuestion = false;
        this.questions[index].id = savedQuestion.id;
        return savedQuestion;
      }),
      concatMap((d, i) => {
        return this._quizService.adminAssignRelatedQuestion(this.quiz.id, d.id)
        .pipe(map(() => {
          return d;
        }))
      })
    )
    }
    return this._questionService.adminUpdateQuestion(this.questions[index], fileToPush);
  }

  removeQuestionAt(index: number) {
    if (!this.isCreateNew) {
      let question = this.questions[index];
      if (!question.isNewQuestion) {
        this.deleteQuestionIds.push(question.id);
      }
    }
    this.questions = this.questions.filter((d, i) => i != index);
  }

  private showSuccess(){
    this._translate.get("Commons.Messages.Success").subscribe(d => {
        this._toastr.success(d);
    })
  }

  private showError(){
    this._translate.get("Admin.Question.CorrectErrorsFirstToContinue").subscribe(d => {
        this._toastr.error(d);
    })
  }

  isSingleChoiceQuestion(question: Question): question is SingleChoiceQuestion {
    return question.type === QuestionType.SingleChoice;
  }

  isMultiChoiceQuestion(question: Question): question is MultiChoiceQuestion {
    return question.type === QuestionType.MultiChoice;
  }

  isImageSingleChoiceQuestion(question: Question): question is ImageSingleChoiceQuestion {
    return question.type === QuestionType.ImageSingleChoice;
  }

  isImageMultiChoiceQuestion(question: Question): question is ImageMultiChoiceQuestion {
    return question.type === QuestionType.ImageMultiChoice;
  }

  isBinaryQuestion(question: Question): question is BinaryQuestion {
    return question.type === QuestionType.Binary;
  }

  isEssayQuestion(question: Question): question is EssayQuestion {
    return question.type === QuestionType.Essay;
  }
}
