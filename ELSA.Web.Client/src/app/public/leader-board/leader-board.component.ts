import { Component, OnDestroy, OnInit } from '@angular/core';
import { SignalRService } from '../../../services/base/signalR.services';
import { LeaderBoardItem } from '../../../models/leader-boards/leader-board.model';

@Component({
  selector: 'leader-board',
  templateUrl: './leader-board.component.html',
  styleUrl: './leader-board.component.scss'
})
export class LeaderBoardComponent implements OnInit, OnDestroy {
  signalRMethodName: string = "LeaderBoardChange";
  leaderBoardItems: LeaderBoardItem[] = [];
  constructor(private _signalRService: SignalRService){

  }
  ngOnDestroy(): void {
    this._signalRService.deregister(this.signalRMethodName);
  }
  ngOnInit(): void {
    this._signalRService.register<LeaderBoardItem[]>(this.signalRMethodName, (data) => {
      this.leaderBoardItems = data;
    });
  }
}
