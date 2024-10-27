
import { PublicLayoutComponent } from "./public-layout/public-layout.component";
import { SharedModule } from "../shared.module";
import { TranslateModule } from "@ngx-translate/core";
import { NgModule } from "@angular/core";
import { QuizComponent } from "./quiz/quiz.component";
import { TakeQuizComponent } from "./take-quiz/take-quiz.component";
import { SingleChoiceQuestionComponent } from "./question-component/single-choice-question/single-choice-question.component";
import { BinaryQuestionComponent } from "./question-component/binary-question/binary-question.component";
import { MultiChoiceQuestionComponent } from "./question-component/multi-choice-question/multi-choice-question.component";
import { EssayQuestionComponent } from "./question-component/essay-question/essay-question.component";
import { ImageMultiChoiceQuestionComponent } from "./question-component/image-multi-choice-question/image-multi-choice-question.component";
import { ImageSingleChoiceQuestionComponent } from "./question-component/image-single-choice-question/image-single-choice-question.component";
import { QuizListComponent } from './quiz-list/quiz-list.component';
import { HomePageComponent } from './home-page/home-page.component';
import { ProgressionComponent } from './progression/progression.component';
import { ScoreComponent } from './score/score.component';
import { LeaderBoardComponent } from './leader-board/leader-board.component';
import { StartPageComponent } from './start-page/start-page.component';
import { MePageComponent } from './me-page/me-page.component';
import { HTTP_INTERCEPTORS } from "@angular/common/http";
import { AnonymousUserInterceptor } from "../../interceptors/anonymous-intercepter";


@NgModule({
  declarations: [
    PublicLayoutComponent,
    QuizComponent,
    TakeQuizComponent,
    BinaryQuestionComponent,
    MultiChoiceQuestionComponent,
    SingleChoiceQuestionComponent,
    ImageMultiChoiceQuestionComponent,
    ImageSingleChoiceQuestionComponent,
    EssayQuestionComponent,
    QuizListComponent,
    HomePageComponent,
    ProgressionComponent,
    ScoreComponent,
    LeaderBoardComponent,
    StartPageComponent,
    MePageComponent,
  ],
  imports:[
    TranslateModule.forChild(),
    SharedModule
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AnonymousUserInterceptor, multi: true },
  ]
})
export class PublicModule { }
