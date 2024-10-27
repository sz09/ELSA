import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PublicLayoutComponent } from './public-layout/public-layout.component';
import { TakeQuizComponent } from './take-quiz/take-quiz.component';
import { QuizListComponent } from './quiz-list/quiz-list.component';
import { HomePageComponent } from './home-page/home-page.component';
import { StartPageComponent } from './start-page/start-page.component';
import { MePageComponent } from './me-page/me-page.component';
import { AnonymousAuthGuard } from '../../services/auth.service';
const routes: Routes = [
  {
    path: '',
    pathMatch: "prefix",
    component: PublicLayoutComponent,
    children: [
      { path: 'home', component: HomePageComponent },
      { path: 'start', component: StartPageComponent },
      { path: 'quizzes', component: QuizListComponent },
      { path: 'me', component: MePageComponent, canActivate: [AnonymousAuthGuard] },
      { path: 'quiz/:quizId', component: TakeQuizComponent, canActivate: [AnonymousAuthGuard] }
    ],

  }
]
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PublicRoutingModule { }
