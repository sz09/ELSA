import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { map, Observable } from "rxjs";
import { AuthService } from "../services/auth.service";
@Injectable()
export class AnonymousUserInterceptor implements HttpInterceptor {
    constructor(private _auth: AuthService) { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        var anonymousUser = this._auth.loadAnonymousUserFromLocal();
        if (anonymousUser) {
            request = request.clone({
                setHeaders: {
                    "username": anonymousUser.username,
                    "userId": anonymousUser.id
                }
            })
        }
        return next.handle(request).pipe(map(response => {
            return response;
        }))
    }
}