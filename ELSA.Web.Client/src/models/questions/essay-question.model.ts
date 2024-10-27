import { QuestionType } from "../../enums/question.type";
import { Question } from "./base/base-question.model";

export class EssayQuestion extends Question {
    constructor(id: string){
        super(id, QuestionType.Essay);
    }
    
    correctAnswer!: string;
    ignoreCase: boolean = true;
    anyAnswer: boolean = false;
}