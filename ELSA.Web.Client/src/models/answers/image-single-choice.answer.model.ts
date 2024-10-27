import { QuestionType } from "../../enums/question.type";
import { Answer } from "./base/base-answer.model";

export class ImageSingleChoiceAnswer extends Answer {
    constructor(id: string){
        super(id, QuestionType.ImageSingleChoice);
    }
    
    answers!: string[];
}