import { QuestionType } from "../../enums/question.type";
import { Question } from "./base/base-question.model";

export class MultiChoiceQuestion extends Question {
    constructor(id: string){
        super(id, QuestionType.MultiChoice);
    }
    
    options: string[] = [];
    correctAnswers: number[] = [];
}