import { QuestionType } from "../../enums/question.type";
import { Question } from "./base/base-question.model";

export class ImageMultiChoiceQuestion extends Question {
    constructor(id: string){
        super(id, QuestionType.ImageMultiChoice);
    }

    options: string[] = [];
    correctAnswers: number[] = [];
    newFiles: { index: number, file: File}[] = [];
}