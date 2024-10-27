import { Injector, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { QuizComponent } from './public/quiz/quiz.component';
import { TakeQuizComponent } from './public/take-quiz/take-quiz.component';
import { MissingTranslationHandler, MissingTranslationHandlerParams, TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { defaultLanguage } from '../const/const';
import { HttpClient } from '@angular/common/http';
import { AdminModule } from './admin/admin.module';
import { PublicModule } from './public/public.module';
import { ToastrModule } from 'ngx-toastr';

export function createTranslateLoader(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/i18n/', '.json');
}
export class SharedMissingTranslationHandler implements MissingTranslationHandler {
  handle(params: MissingTranslationHandlerParams) {
    return params.key;
  }
}

export let AppInjector: Injector;
@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    AdminModule,
    PublicModule,
    BrowserModule,
    AppRoutingModule,
    TranslateModule.forRoot({
      defaultLanguage: defaultLanguage,
      missingTranslationHandler: { provide: MissingTranslationHandler, useClass: SharedMissingTranslationHandler },
      loader: {
        provide: TranslateLoader,
        useFactory: (createTranslateLoader),
        deps: [HttpClient]
      }
    }),
    
    ToastrModule.forRoot({
      positionClass: 'toast-bottom-right',
      preventDuplicates: true,
    }),
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule {
  constructor(private injector: Injector) {
    AppInjector = this.injector;
  }
}
