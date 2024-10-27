import { Injectable } from "@angular/core";
import { BaseService } from "./base/base-service";
import { Observable } from "rxjs";
import { Question } from "../models/questions/base/base-question.model";
import { HttpClient } from "@angular/common/http";
@Injectable({
    providedIn: 'root'
})
export class QuestionService extends BaseService {
    constructor(private _http: HttpClient) {
        super();
    }

    public loadQuestionFromQuiz(quizId: string): Observable<Question[]> {
        return this._http.get<Question[]>(`${this.host}/api/v${this.API_VERSION}/questions/from-quiz/${quizId}`,
            this.getRequestHeaders());
    }

    public answerQuestion(questionId: string, answer: any): Observable<boolean>{
        return this._http.put<boolean>(`${this.host}/api/v${this.API_VERSION}/questions/answers`,
            {
                "questionId": questionId,
                "answer": answer
            },
            this.getRequestHeaders());
    }

    public adminLoadQuestionFromQuiz(quizId: string): Observable<Question[]> {
        return this._http.get<Question[]>(`${this.host}/api/admin/v${this.API_VERSION}/questions/from-quiz/${quizId}`,
            this.getRequestHeaders());
    }


    public adminCreateQuestion(question: Question, files: {index: number, file: File}[] = null): Observable<Question> {
        const formData = this.toFormData(question, files);
        return this._http.post<Question>(`${this.host}/api/admin/v${this.API_VERSION}/questions`,
            formData,
            this.getRequestHeadersWithoutContentTypes());
    }

    public adminUpdateQuestion(question: Question, files: {index :number, file: File}[] = null): Observable<Question> {
        const formData = this.toFormData(question, files);
        return this._http.put<Question>(`${this.host}/api/admin/v${this.API_VERSION}/questions`,
            formData,
            this.getRequestHeadersWithoutContentTypes());
    }

    private toFormData(question: any, files: {index :number, file: File}[] = null){
        const formData = new FormData();
        if(files && files.length){
            files.forEach((file, i) => {
                formData.append(`${file.index}`, file.file, file.file.name);
            })
        }
        
        var keys = Object.keys(question);
        for(let key of keys) {
            formData.append(key, question[key]);
        }
        return formData;
    }
}