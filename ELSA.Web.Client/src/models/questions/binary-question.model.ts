import { QuestionType } from "../../enums/question.type";
import { Question } from "./base/base-question.model";

export class BinaryQuestion extends Question {
    constructor(id: string){
        super(id, QuestionType.Binary);
    }

    correctAnswer!: boolean;
}