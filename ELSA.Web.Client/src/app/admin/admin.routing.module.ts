import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { AuthAdminGuard } from "../../services/auth.service";
import { AdminLayoutComponent } from "./admin-layout/admin-layout.component";
import { QuizListComponent } from "./quiz-list/quiz-list.component";
import { QuizComponent } from "./quiz/quiz.component";
const routes: Routes = [
  {
    path: '',
    pathMatch: "prefix",
    component: AdminLayoutComponent, children: [
      { path: 'quizzes', component: QuizListComponent, canActivate: [AuthAdminGuard] },
      { path: 'quiz/create', component: QuizComponent, canActivate: [AuthAdminGuard] },
      { path: 'quiz/:id', component: QuizComponent, canActivate: [AuthAdminGuard] },
    ],
    canActivate: [AuthAdminGuard]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AdminRoutingModule { }
