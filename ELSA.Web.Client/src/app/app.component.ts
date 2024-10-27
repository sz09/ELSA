import { Component, OnInit } from '@angular/core';
import { SignalRService } from '../services/base/signalR.services';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent  implements OnInit{
  ngOnInit(): void {
  }
  title = 'ELSA.Web.Client';
}
