import { NgModule } from "@angular/core";
import { SharedModule } from "../shared.module";
import { AdminLayoutComponent } from "./admin-layout/admin-layout.component";
import { LoginComponent } from "../login/login.component";
import { TranslateModule } from "@ngx-translate/core";
import { HTTP_INTERCEPTORS } from "@angular/common/http";
import { LoaderInterceptor } from "../../interceptors/loader-intercepter";
import { QuizComponent } from './quiz/quiz.component';
import { QuizListComponent } from './quiz-list/quiz-list.component';
import { BinaryQuestionComponent } from "./question-component/binary-question/binary-question.component";
import { MultiChoiceQuestionComponent } from "./question-component/multi-choice-question/multi-choice-question.component";
import { SingleChoiceQuestionComponent } from "./question-component/single-choice-question/single-choice-question.component";
import { ImageMultiChoiceQuestionComponent } from "./question-component/image-multi-choice-question/image-multi-choice-question.component";
import { ImageSingleChoiceQuestionComponent } from "./question-component/image-single-choice-question/image-single-choice-question.component";
import { EssayQuestionComponent } from "./question-component/essay-question/essay-question.component";
import { RouterModule } from "@angular/router";
import { PaginationComponent } from "./pagination/pagination.component";
import { JwtInterceptor } from "../../interceptors/jwt-interceptor";
import { ConfirmDialogComponent } from "./confirm-dialog/confirm-dialog.component";
import { NgLabelTemplateDirective, NgOptionTemplateDirective, NgSelectComponent, NgSelectModule } from "@ng-select/ng-select";


@NgModule({
  declarations: [
    AdminLayoutComponent,
    EssayQuestionComponent,
    BinaryQuestionComponent,
    MultiChoiceQuestionComponent,
    SingleChoiceQuestionComponent,
    ImageMultiChoiceQuestionComponent,
    ImageSingleChoiceQuestionComponent,
    PaginationComponent,
    ConfirmDialogComponent,
    QuizComponent,
    QuizListComponent,
    LoginComponent
  ],
  imports:[
    NgSelectModule,
    NgLabelTemplateDirective,
    NgOptionTemplateDirective,
    NgSelectComponent,
    TranslateModule.forChild(),
    SharedModule
  ],
  exports: [RouterModule],
  providers: [
    {provide: HTTP_INTERCEPTORS, useClass: LoaderInterceptor, multi: true},
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
  ]
})
export class AdminModule { }
