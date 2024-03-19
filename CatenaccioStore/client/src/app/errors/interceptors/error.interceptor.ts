import { Injectable } from "@angular/core";
import{
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
}
 from '@angular/common/http'
import { Observable, catchError, throwError } from "rxjs";
import { Router } from "@angular/router";
import { ToastrService } from "ngx-toastr";
import { MatSnackBar } from "@angular/material/snack-bar";

@Injectable()
export class ErrorInterceptor implements HttpInterceptor{
  constructor(private router: Router, private snackbar: MatSnackBar){}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {
        if(error){
          if(error.status === 400){
            if(error.error.errors){
              throw error.error;
            }
            else
              this.showError(error.error.message, error.status.toString())
          }
          if(error.status === 401)
            this.showError(error.error.message, error.status.toString())
          if(error.status === 404){
            this.router.navigateByUrl('/not-found')
          }
          if(error.status === 500){
            this.showError(error.error.message, error.status.toString())
          }
        }
        return throwError(() => new Error(error.message))
      })
    )
  }
  showError(errorMessage: string, code : string) {
    this.snackbar.open(errorMessage, code, {
      duration: 3000
    });
  }
}