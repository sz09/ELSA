import { QuestionType } from "../../enums/question.type";
import { Answer } from "./base/base-answer.model";

export class ImageMultiChoiceAnswer extends Answer {
    constructor(id: string){
        super(id, QuestionType.ImageMultiChoice);
    }

    answer!: string[];
}