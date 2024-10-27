import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { map, Observable } from "rxjs";
import { hideLoading, showLoading } from "../services/loader.service";
@Injectable()
export class LoaderInterceptor implements HttpInterceptor {
    constructor() { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        showLoading();
        return next.handle(request).pipe(map(response => {
            hideLoading();
            return response;
        }))
    }
}