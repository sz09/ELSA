import { QuestionType } from "../../enums/question.type";
import { Answer } from "./base/base-answer.model";
// TODO: work on it when have time 
export class OrderingAnswer extends Answer {
    constructor(id: string){
        super(id, QuestionType.Ordering);
    }
}