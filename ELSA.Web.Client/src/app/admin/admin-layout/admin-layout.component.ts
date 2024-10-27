import { Component, OnInit } from "@angular/core";
import { currentLanguageKey, defaultLanguage } from "../../../const/const";
import { TranslateService } from "@ngx-translate/core";
import { AuthService } from "../../../services/auth.service";

@Component({
  selector: 'app-layout',
  templateUrl: './admin-layout.component.html',
  styleUrl: './admin-layout.component.scss'
})
export class AdminLayoutComponent implements OnInit {
  constructor(private _translate: TranslateService, private _authService: AuthService) { }
  ngOnInit(): void {
    var currentLang = localStorage.getItem(currentLanguageKey);
    if(!currentLang){
      this.onSelectLanguage(defaultLanguage);
    }
    else if(!this.supportLanguages.filter(d => d.code == currentLang)){ // Check is in supported language
      this.onSelectLanguage(defaultLanguage);
    }
    else {
      this.onSelectLanguage(currentLang);
    }
  }
  supportLanguages: { code: string, displayKey: string, css: string }[] = [
    {
      code: 'en',
      displayKey: 'Langs.En_GB',
      css: 'flag-united-kingdom flag'
    },
    {
      code: 'vn',
      displayKey: 'Langs.vi_VN',
      css: 'flag-vietnam flag'
    }
  ];

  get displayUser(){
    return '';
  } 
  isSelectedLanguage(code: string) {
    return this._translate.currentLang == code;
  }
  onSelectLanguage(code: string) {
    this._translate.use(code).subscribe(d => {
      localStorage.setItem(currentLanguageKey, code);
    })
  }
  isInPage(route: string) {
    return route === window.location.href.replace(window.location.origin, '');
  }

  get isLogIn(){
    return this._authService.isLoggedIn();
  }
  
  logout(){
    this._authService.logout();
  }
}