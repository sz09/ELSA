import { Component, OnInit } from '@angular/core';
import { UserInfomartion } from '../../../models/user-infos/user-information.model';
import { AuthService } from '../../../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-start-page',
  templateUrl: './start-page.component.html',
  styleUrl: './start-page.component.scss'
})
export class StartPageComponent implements OnInit {
  constructor(private _authService: AuthService, private _router: Router) {

  }
  ngOnInit(): void {
    var user = this._authService.loadAnonymousUserFromLocal();
    if(user){
      this._router.navigateByUrl('/quizzes');
    }
  }
  userInfomartions: UserInfomartion = new UserInfomartion();
  go(){
    this._authService.registerUserAnonymous(this.userInfomartions.username, this.userInfomartions.email).subscribe(d => {
      this._router.navigateByUrl('/quizzes');
    })
  }
}
