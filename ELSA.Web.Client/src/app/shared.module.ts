import { CommonModule } from "@angular/common";
import { provideHttpClient, withInterceptorsFromDi } from "@angular/common/http";
import { BrowserModule } from "@angular/platform-browser";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { FormsModule } from "@angular/forms";
import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";
import { NotFoundComponent } from "./not-found/not-found.component";

@NgModule({
    declarations: [
        NotFoundComponent
    ],
    exports: [
        BrowserAnimationsModule,
        FormsModule,
        CommonModule,
        BrowserModule,
        RouterModule,
        NotFoundComponent
    ],
    imports: [
        CommonModule,
        BrowserModule,
        FormsModule,
        BrowserAnimationsModule,
        RouterModule 
    ],
    providers: [
        provideHttpClient(withInterceptorsFromDi())
    ]
})
export class SharedModule { }
