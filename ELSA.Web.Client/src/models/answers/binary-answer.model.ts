import { QuestionType } from "../../enums/question.type";
import { Answer } from "./base/base-answer.model";

export class BinaryAnswer extends Answer {
    constructor(id: string){
        super(id, QuestionType.Binary);
    }

    answer!: boolean;
}