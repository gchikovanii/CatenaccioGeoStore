import { Injectable } from "@angular/core";
import{
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
}
 from '@angular/common/http'
import { Observable, catchError, delay, finalize, throwError } from "rxjs";

import { BusyService } from "../../services/busy.service";

@Injectable()
export class LoadingInterceptor implements HttpInterceptor{
  constructor(private busyService: BusyService){}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if(req.url.includes('emailexists') || req.method==='POST' && req.url.includes('ordersS')){
      return next.handle(req);
    }
    this.busyService.busy();
    return next.handle(req).pipe(
        delay(1000),
        finalize(() => this.busyService.idle())
    )
  }
}
