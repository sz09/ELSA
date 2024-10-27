import { Directive, EventEmitter, Input, Output } from "@angular/core";
import { TranslateService } from "@ngx-translate/core";

@Directive()
export abstract class QuestionComponentBase<T> {
    translateValues = {
        PleaseGiveAnAnswer: ''
    };
    
    abstract setQuestionInfos(): void;
    errorMessage: string;
    constructor(protected _translate: TranslateService) {
        _translate.get("Admin.Question.PleaseProvideValidOptions_Error").subscribe(d => {
            this.translateValues.PleaseGiveAnAnswer = d;
        })
        
    }
    collectAnswer(): T {
        if(!this.validate()){
            return null;
        }
        return this.correctAnswer;
    }

    @Input()
    correctAnswer: T;

    validate(): boolean{
        if(this.correctAnswer === null || typeof(this.correctAnswer) === 'undefined'){
            this.showWarning();
            return false;
        }

        return true;
    }
    
    protected showWarning(){
        this.errorMessage = this.translateValues.PleaseGiveAnAnswer;
    }
}
