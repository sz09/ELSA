import { QuestionType } from "../../enums/question.type";
import { Answer } from "./base/base-answer.model";

export class SingleChoiceAnswer extends Answer {
    constructor(id: string){
        super(id, QuestionType.MultiChoice);
    }
    
    correctAnswers!: string[];
}