export enum AnswerState {
    NotAnswered,
    Incorrect,
    Correct
}


export class QuestionState {
    state!: AnswerState;
    index!: number;
    questionId!: string;
    points!: number;
}