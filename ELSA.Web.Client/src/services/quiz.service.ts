import { Injectable } from "@angular/core";
import { BaseService } from "./base/base-service";
import { Observable } from "rxjs";
import { HttpClient } from "@angular/common/http";
import { Direction, ORDER_BY_DEFAULT, PAGE_DEFAULT, PAGE_SIZE_DEFAULT, PageResult } from "../utilities/page.utilities";
import { Quiz } from "../models/quiz/quiz.model";
@Injectable({
    providedIn: 'root'
})
export class QuizService extends BaseService {
    constructor(private _http: HttpClient) {
        super();
    }

    public fetchQuizzes(orderBy: string, direction: Direction, page: number = PAGE_DEFAULT, pageSize = PAGE_SIZE_DEFAULT): Observable<PageResult<Quiz>> {
        return this._http.post<PageResult<Quiz>>(`${this.host}/api/v${this.API_VERSION}/quizzes`,
            this.getBodyData(orderBy, direction, page, pageSize),
            this.getRequestHeaders());
    }

    public getQuizScoredQuestions(quizId: string): Observable<string[]> {
        return this._http.get<string[]>(`${this.host}/api/v${this.API_VERSION}/quizzes/quiz-scored-questions/${quizId}`,
            this.getRequestHeaders());
    }

    public adminFetchQuizzes(orderBy: string, direction: Direction, page: number = PAGE_DEFAULT, pageSize = PAGE_SIZE_DEFAULT): Observable<PageResult<Quiz>> {
        return this._http.post<PageResult<Quiz>>(`${this.host}/api/admin/v${this.API_VERSION}/quizzes/fetch`,
            this.getBodyData(orderBy, direction, page, pageSize),
            this.getRequestHeaders());
    }

    public adminLoadQuiz(quizId: string): Observable<Quiz>{
        return this._http.get<Quiz>(`${this.host}/api/admin/v${this.API_VERSION}/quizzes/${quizId}`,
            this.getRequestHeaders());
    }

    public adminCreateQuiz(model: Quiz): Observable<Quiz>{
        return this._http.post<Quiz>(`${this.host}/api/admin/v${this.API_VERSION}/quizzes`,
            model,
            this.getRequestHeaders());
    }
    
    public adminUpdateQuiz(model: Quiz): Observable<Quiz>{
        return this._http.put<Quiz>(`${this.host}/api/admin/v${this.API_VERSION}/quizzes`,
            model,
            this.getRequestHeaders());
    }
    
    public adminDeleteQuiz(quizId: string): Observable<Quiz>{
        return this._http.delete<Quiz>(`${this.host}/api/admin/v${this.API_VERSION}/quizzes/${quizId}`,
            this.getRequestHeaders());
    }
    
    public adminAssignRelatedQuestion(quizId: string, questionId: string): Observable<Quiz>{
        return this._http.put<Quiz>(`${this.host}/api/admin/v${this.API_VERSION}/quizzes/assign-related-question/${quizId}/${questionId}`,
            null,
            this.getRequestHeaders());
    }
}