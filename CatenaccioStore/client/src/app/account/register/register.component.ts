import { Component } from '@angular/core';
import { AbstractControl, AsyncValidatorFn, FormBuilder, Validators } from '@angular/forms';
import { AccountService } from '../../services/account.service';
import { Router } from '@angular/router';
import { debounceTime, finalize, map, switchMap, take } from 'rxjs';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {
  errors: string[] | null = null;
  constructor(private fb: FormBuilder, private accountService: AccountService, private router:Router){}
  complexPassword = /^(?=.*[!@#$%^&*()_+{}\[\]:;<>,.?\/\\~-])(?=.*[A-Z])(?=.*[a-z])(?=.*\d).{8,}$/;
  registerForm = this.fb.group({
    displayName: ['',Validators.required],
    email: ['',[Validators.required,Validators.email], [this.validateEmailNotTaken()]],
    password: ['',[Validators.required,Validators.pattern(this.complexPassword)]],
  })

  validateEmailNotTaken() : AsyncValidatorFn{
    return (control: AbstractControl) => {
      return control.valueChanges.pipe(
        debounceTime(1500),
        take(1),
        switchMap(() => {
          return this.accountService.checkEmailExists(control.value).pipe(
            map(result => result ? {emailExists: true} : null),
            finalize(()=>control.markAsTouched())
          )
        })
      )
    }
  }

  onSubmit(){
    this.accountService.register(this.registerForm.value).subscribe({
      next: () => this.router.navigateByUrl('/login'),
      error: error => this.errors = error.errors
    })
  }

  
}
