import { QuestionType } from "../../../enums/question.type";
import { BaseModel } from "../../base/base.model";

export abstract class Question extends BaseModel{
    constructor(id: string, type: QuestionType) {
        super();
        this.id = id;
        this.type = type;
    }
    isNewQuestion: boolean = true;
    points: number = 0;
    type!: QuestionType;
    question!: string;
}

