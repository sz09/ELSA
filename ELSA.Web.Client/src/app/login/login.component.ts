import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { hideLoading } from '../../services/loader.service';
import { LoginModel } from '../../models/auth/login.model';

@Component({
  selector: 'login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  loginModel: LoginModel = new LoginModel( '', '', false);
  isError: boolean = false;
  showPassword: boolean = false;
  constructor(private _authService: AuthService,
    private _activedRoute: ActivatedRoute,
    private _router: Router) {
  }
    ngOnInit(): void {
      hideLoading();
    }

  doLogin() {
    this.isError = false;
    this._authService.login(this.loginModel).subscribe(d => {
      if (d) {
        this._activedRoute.queryParams.subscribe(d => {
          let navigateRoute  = d['returnUrl'] ?? 'home';
          this._router.navigateByUrl(navigateRoute);
        })
      }
      else {
        this.isError = true;
      }
    })
  }
}
