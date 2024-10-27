import { BaseModel } from "../base/base.model";
import { Question } from "../questions/base/base-question.model";

export class Quiz extends BaseModel {
    subject!: string;
    
    questionIds!: string[];
    questions!: Question[];
}