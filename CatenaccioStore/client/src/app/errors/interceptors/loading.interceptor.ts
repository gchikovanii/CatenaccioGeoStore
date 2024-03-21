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
import { Router } from "@angular/router";
import { MatSnackBar } from "@angular/material/snack-bar";
import { BusyService } from "../../services/busy.service";

@Injectable()
export class LoadingInterceptor implements HttpInterceptor{
  constructor(private busyService: BusyService){}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if(!req.url.includes('emailexists')){
      this.busyService.busy();
      console.log("hit and run")
    }
    return next.handle(req).pipe(
        delay(1000),
        finalize(() => this.busyService.idle())
    )
  }
}
