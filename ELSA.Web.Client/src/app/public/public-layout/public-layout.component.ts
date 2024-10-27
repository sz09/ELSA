import { Component, OnDestroy, OnInit } from '@angular/core';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-public-layout',
  templateUrl: './public-layout.component.html',
  styleUrl: './public-layout.component.scss'
})
export class PublicLayoutComponent implements OnInit, OnDestroy{
  username: string;
  constructor(private _auth: AuthService){

  }
  ngOnDestroy(): void {
    this._auth.unRegisterOnUserUpdate("UpdateLayoutUserName");
  }
  ngOnInit(): void {
    this._auth.registerOnUserUpdate("UpdateLayoutUserName", (d) => {
      this.username = d.username;
    });
    this._auth.loadAnonymousUserFromLocal();
  }
}
