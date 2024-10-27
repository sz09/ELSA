import { QuestionType } from "../../enums/question.type";
import { Answer } from "./base/base-answer.model";

export class EssayAnswer extends Answer {
    constructor(id: string){
        super(id, QuestionType.Essay);
    }

    anwser!: string;
}