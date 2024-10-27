import { Directive, EventEmitter, Input, input, Output } from "@angular/core";
import { TranslateService } from "@ngx-translate/core";
import { ToastrService } from "ngx-toastr";

@Directive()
export abstract class QuestionComponentBase<T> {
    translateValues = {
        PleaseGiveAnAnswer: ''
    };
    constructor(protected _toastrService: ToastrService, protected _translate: TranslateService) {
        _translate.get("TakeQuiz.PleaseGiveAnAnswer").subscribe(d => {
            this.translateValues.PleaseGiveAnAnswer = d;
        })
        
    }
    collectAnswer(): void {
        if(!this.validate()){
            return;
        }

        this.onCollectedEmitter.emit(this.answer);
    }
    @Output()
    onCollectedEmitter: EventEmitter<T> = new EventEmitter<T>();
    @Input()
    answer: T;

    protected validate(): boolean{
        if(this.answer === null || typeof(this.answer) === 'undefined'){
            this.showWarning();
            return false;
        }

        return true;
    }
    
    protected showWarning(){
        this._toastrService.warning(this.translateValues.PleaseGiveAnAnswer);
    }
}