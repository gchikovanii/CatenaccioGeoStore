import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { log } from 'console';
import { AccountService } from '../../services/account.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent implements OnInit{
  loginForm = new FormGroup({
    email : new FormControl('', [Validators.required, Validators.email]),
    password : new FormControl('', Validators.required),
  })
  constructor(private accountService : AccountService, private router: Router){
  }

  ngOnInit(): void {
  }

  onSubmit(){
    this.accountService.login(this.loginForm.value).subscribe({
      next: () => this.router.navigateByUrl('/shop')
    })
  }
}
