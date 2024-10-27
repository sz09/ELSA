import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { _loginRoute, AuthService } from "../services/auth.service";
import { Router } from "@angular/router";
import { ToastrService } from "ngx-toastr";
import { catchError, from, Observable, switchMap, throwError } from "rxjs";
import { AppInjector } from "../app/app.module";
import { TranslateService } from "@ngx-translate/core";
import { hideLoading } from "../services/loader.service";

@Injectable()
export class JwtInterceptor implements HttpInterceptor {
    constructor(private _authService: AuthService, private router: Router, private toastr: ToastrService) { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        const user = this._authService.getUser();
        if (user && user.accessToken) {
            request = request.clone({
                setHeaders: { Authorization: `Bearer ${user.accessToken}` }
            });
        }
        return next.handle(request).pipe(catchError(err => {
            if (err.status === 401) {
                return from(this._authService.refreshToken(user.refreshToken)).pipe(
                    switchMap((newToken: string) => {
                        const newAuthReq = request.clone({
                            setHeaders: {
                                Authorization: `Bearer ${newToken}`
                            }
                        });
                        return next.handle(newAuthReq);
                    }),
                    catchError((err) => {
                        this.handleLogout();
                        return throwError(err);
                    })
                );
            }
            if (err.status == 403 && this._authService.getUser()) {
                this.handleLogout();
            }
            else if ([500, 400].includes(err.status)) {
                this.getErrorMessage(err).subscribe(d => {
                    this.toastr.error(d.message, d.title, {
                        timeOut: 5000,
                        closeButton: false
                    })
                })
            }
            const error = (err && err.error) || err.statusText;
            hideLoading();
            return throwError(() => error);
        })
        );
    }

    private getErrorMessage(err: HttpErrorResponse): Observable<ErrorMessage> {
        return new Observable<ErrorMessage>(
            (obs) => {
                var translateService = AppInjector.get(TranslateService);
                var keysToTranslate = [
                    'Auth.Common_Error',
                ]
                if (err.status == 400) {
                    keysToTranslate.push(`Errors.${err.error}`);
                }
                else {
                    keysToTranslate.push('Auth.Common_Error_InternalServerError');
                }

                translateService.get(keysToTranslate).subscribe(d => {
                    var keys = Object.keys(d);
                    var errorMessage = new ErrorMessage();
                    errorMessage.title = d[keys[0]]; 
                    errorMessage.message = d[keys[1]];
                    obs.next(errorMessage);
                });
            }
        )
    }
    private handleLogout() {
        this._authService.logout();
        this.router.navigate([_loginRoute], { queryParams: { returnUrl: this.router.url } })
    }
}

export class ErrorMessage {
    message: string;
    title: string;
}