import { QuestionType } from "../../../enums/question.type";
import { BaseModel } from "../../base/base.model";

export abstract class Answer extends BaseModel {
    constructor(id: string, type: QuestionType) {
        super();
        this.id = id;
        this.type = type;
    }

    type!: QuestionType;
}

