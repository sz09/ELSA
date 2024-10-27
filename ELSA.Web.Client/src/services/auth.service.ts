import { HttpClient } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Router, RouterStateSnapshot } from "@angular/router";
import { map, Observable } from "rxjs";
import { DOCUMENT } from "@angular/common";
import { BaseService } from "./base/base-service";
import { LoginModel } from "../models/auth/login.model";
import { SignupModel } from "../models/auth/sign-up.model";
import { jwtDecode } from "jwt-decode";

export const _loginRoute = 'login';
export const _homeRoute = '';
export const _userKey = 'i_user';
export const _anonymousUserKey = 'i_anonymousUser';
@Injectable({
  providedIn: 'root'
})
export class AuthService extends BaseService {
  constructor(private _http: HttpClient, private _router: Router, @Inject(DOCUMENT) private document: Document) {
    super();
  }
_localStorage = this.document.defaultView?.localStorage;

  login(loginModel: LoginModel): Observable<boolean> {
    return this._http.post<User>(`${this.host}/account/login`, loginModel, this.getRequestHeaders())
      .pipe(map(d => this.processToken(d)));
  }

  refreshToken(token: string): Observable<string> {
    return this._http.post<User>(`${this.host}/account/refresh-token`, {
      'token': token
    }, this.getRequestHeaders())
      .pipe(map(d => {
        this.processToken(d);
        return d.accessToken;
      }));
  }

  private processToken(user: User){
    if (user && user.accessToken) {
      this._localStorage!.setItem(_userKey, JSON.stringify(user));
      return true;
    }
    return false;
  }
  
  signUp(signupModel: SignupModel): Observable<void> {
    return this._http.post<any>(`${this.host}/users/register`, signupModel, this.getRequestHeaders());
  }

  logout() {
    this._localStorage!.removeItem(_userKey);
    this._router.navigateByUrl(_loginRoute);
  }

  getUser() {
    var userJson = this._localStorage!.getItem(_userKey);
    if (userJson) {
      return JSON.parse(userJson) as User;
    }
    return null;
  }
  
  getUserInfos(): UserInfos {
    var user = this.getUser();
    if (user) {
      return jwtDecode(user.accessToken) as UserInfos;
    }

    return null;
  }

  get isAdmin() {
    return this.getUserInfos()?.role.indexOf("ELSA.API.ADMIN") > -1;
  }

  registerUserAnonymous(username: string, email: string): Observable<void>{
    return this._http.post<string>(`${this.host}/users/register-user-anonymous`, {
      'username': username,
      'email': email
    }, this.getRequestHeaders())
    .pipe(map(id => {
      let user: AnonymousUserInfos = {
        id: id,
        username: username,
        email: email
      }
      this._localStorage!.setItem(_anonymousUserKey, JSON.stringify(user));
      this.notifyHandler(user);
    }))
  }
  notifyHandler(user: AnonymousUserInfos){
    Object.values(this.userUpdateHandlers).forEach(d => {
      d(user);
    })
  }
  loadAnonymousUserFromLocal(): AnonymousUserInfos{
    let userJson =  this._localStorage!.getItem(_anonymousUserKey);
    if(userJson){
      let user = JSON.parse(userJson) as AnonymousUserInfos;
      this.notifyHandler(user);
      return user;
    }

    return null;
  }
  private userUpdateHandlers:  {
    [key: string]: (user: AnonymousUserInfos) => void 
  } = {};
  
  registerOnUserUpdate(key: string, handler: (user: AnonymousUserInfos) => void){
    this.userUpdateHandlers[key] = handler;
  }

  unRegisterOnUserUpdate(key: string) {
    delete this.userUpdateHandlers[key];
  }

  isLoggedIn(){
    return !!this.getUser();
  }
}

@Injectable({ providedIn: 'root' })
export class AnonymousAuthGuard  {
  constructor(
    private router: Router,
    private _authService: AuthService
  ) { }

  canActivate(state: RouterStateSnapshot) {
    if (this._authService.loadAnonymousUserFromLocal()) {
      return true;
    } else {
      // not logged in so redirect to login page with the return url
      this.router.navigate(["start"], { queryParams: { returnUrl: state.url ?? state.root.url } });
      return false;
    }
  }
}



@Injectable({ providedIn: 'root' })
export class AuthAdminGuard  {
  constructor(
    private router: Router,
    private _authService: AuthService
  ) { }

  canActivate(state: ActivatedRouteSnapshot) {
    if (this._authService.isAdmin) {
      return true;
    } else {
      this.router.navigate([_loginRoute],  { queryParams: { returnUrl: window.location.pathname } });
      return false;
    }
  }
}


export class User {
  username!: string;
  accessToken!: string;
  expiryDate!: Date;
  refreshToken!: string;
  refreshTokenIssuedIn!: string;
  refreshTokenLifetime!: number;
  expiresIn!: number;
}

export class UserInfos {
  name!: string;
  role!: string[];
  email!: string;
}
export class AnonymousUserInfos {
  username!: string;
  id!: string;
  email!: string;
}