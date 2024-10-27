import { QuestionType } from "../../enums/question.type";
import { Question } from "./base/base-question.model";

export class SingleChoiceQuestion extends Question {
    constructor(id: string){
        super(id, QuestionType.SingleChoice);
    }

    options: string[] = [];
    correctAnswer!: number;
}