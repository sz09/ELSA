import { QuestionType } from "../../enums/question.type";
import { Question } from "./base/base-question.model";

export class ImageSingleChoiceQuestion extends Question {
    constructor(id: string){
        super(id, QuestionType.ImageSingleChoice);
    }
    
    options: string[] = [];
    newFiles: { index: number, file: File}[] = [];
    correctAnswer!: number;
}