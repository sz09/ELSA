import { Component, OnInit, QueryList, ViewChildren } from '@angular/core';
import { Question } from '../../../models/questions/base/base-question.model';
import { QuestionType } from '../../../enums/question.type';
import { EssayQuestion } from '../../../models/questions/essay-question.model';
import { BinaryQuestion } from '../../../models/questions/binary-question.model';
import { MultiChoiceQuestion } from '../../../models/questions/multi-choice.question.model';
import { SingleChoiceQuestion } from '../../../models/questions/single-choice.question.model';
import { ImageSingleChoiceQuestion } from '../../../models/questions/image-single-choice.question.model';
import { ImageMultiChoiceQuestion } from '../../../models/questions/image-multi-choice.question.model';
import { QuestionService } from '../../../services/question.service';
import { ActivatedRoute } from '@angular/router';
import { AnswerState, QuestionState } from '../../../models/answers/answer-state.enum';
import { QuestionComponentBase } from '../question-component/question.component';
import { ToastrService } from 'ngx-toastr';
import { TranslateService } from '@ngx-translate/core';
import { QuizService } from '../../../services/quiz.service';

@Component({
  selector: 'app-take-quiz',
  templateUrl: './take-quiz.component.html',
  styleUrl: './take-quiz.component.scss'
})
export class TakeQuizComponent implements OnInit {
  constructor(private _questionService: QuestionService,
    private _quizService: QuizService,
    private _activatedRoute: ActivatedRoute,
    private _toast: ToastrService,
    private _translate: TranslateService
  ) { }
  ngOnInit(): void {
    this.loadData();
    this.prepareTranslateData();
  }
  translateValues: {
    Correct: string,
    Incorrect: string
  } = {
      Correct: '',
      Incorrect: ''
    };
  get scoredPoints(): number {
    return this.questionStates.reduce((sum, d) => sum + (d.state === AnswerState.Correct ? d.points : 0), 0);
  }

  get totalScore() {
    return this.questions.reduce((sum, d) => sum + d.points, 0)
  }

  get displayQuestion() {
    return this.questions[this.showQuestion]?.question;
  }
  completedQuestion: number = 0;
  showQuestion: number = 0;
  questions: Question[] = [];
  questionStates: QuestionState[] = [];
  existingAnswers: any[] = [];
  @ViewChildren('questionComponent')
  questionComponents!: QueryList<QuestionComponentBase<any>>

  get canAnswer(): boolean{
    return this.questionStates[this.showQuestion]?.state === AnswerState.NotAnswered;
  }

  private loadData() {
    this._activatedRoute.params.subscribe(d => {
      let quizId = d['quizId'];
      this._questionService.loadQuestionFromQuiz(quizId).subscribe(d => {
        this.questions = d;
        d.forEach((question, i) => {
          this.questionStates.push({
            index: i,
            state: AnswerState.NotAnswered,
            questionId: question.id,
            points: question.points
          });
          this.existingAnswers.push(null);
          this._quizService.getQuizScoredQuestions(quizId).subscribe(d => {
            d.forEach(questionId => {
              let index = this.questionStates.findIndex(e => e.questionId === questionId);
              if (index > -1) {
                this.questionStates[index].state = AnswerState.Correct;
              }
            });
            if (d.length > 0) {
              this.showQuestion = this.questionStates.findIndex(d => d.state === AnswerState.NotAnswered);
            }
          })
        });
      })
    });
  }

  private prepareTranslateData() {
    this._translate.get([
      "TakeQuiz.Correct_Answer",
      "TakeQuiz.Incorrect_Answer"
    ]).subscribe(d => {
      this.translateValues.Correct = d["TakeQuiz.Correct_Answer"];
      this.translateValues.Incorrect = d["TakeQuiz.Incorrect_Answer"];
    })
  }
  previous() {
    if (!this.canPrevious) {
      return;
    }
    
    this.showQuestion--;
  }

  get canPrevious(){
    return this.showQuestion > 0;
  }
  next() {
    if(!this.canNext){
      return;
    }

    this.showQuestion++;
  }

  get canNext(){
      return this.showQuestion < this.questions.length - 1;
  }
  submit() {
    if(!this.canAnswer){
      return;
    }
    if (this.showQuestion >= this.questions.length) {
      return;
    }

    this.questionComponents.forEach(d => d.collectAnswer());
  }

  answerQuestion(answer: any) {
    let question = this.questions[this.showQuestion];
    this._questionService.answerQuestion(question.id, answer).subscribe(isCorrect => {
      this.questionStates[this.showQuestion].state = isCorrect ? AnswerState.Correct : AnswerState.Incorrect;
      if (isCorrect) {
        this._toast.success(this.translateValues.Correct);
        this.next();
      }
      else {
        this._toast.error(this.translateValues.Incorrect);
      }
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

  isDisplayQuestion(i: number) {
    return i === this.showQuestion;
  }
}
