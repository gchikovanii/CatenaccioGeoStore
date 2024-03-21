import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AccountService } from '../services/account.service';
import { ToastrService } from 'ngx-toastr';
import { map } from 'rxjs';

export const authGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const routerService = inject(Router);

  return accountService.currentUserSource$.pipe(
    map(user =>{
      if(user) return true;
      else{
        routerService.navigateByUrl('/');
        return false;
      }
    })
  )
};
