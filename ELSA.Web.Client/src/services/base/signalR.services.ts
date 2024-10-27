import { Injectable } from "@angular/core";
import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import { environment } from "../../environments/environment.development";

@Injectable({
    providedIn: 'root'
})
export class SignalRService {
  private isSignalRConnected: boolean = false;
  private hubConnection?: HubConnection;
  public init() {
    if(this.isSignalRConnected){
      return;
    }

    this.hubConnection = new HubConnectionBuilder()
      .withUrl(`${environment.notificationApi}/ELSAClientHUB`)
      .build();
    this.hubConnection.start().then(d => {
      this.isSignalRConnected = true;
    })
    .catch((err) => {
      console.error(err.toString());
    });
 

    this.hubConnection.onclose((d) => {
      this.isSignalRConnected = false;
    });
  }

  register<T>(methodName: string, handler: (data: T) => void) {
    this.init();
    this.hubConnection.on(methodName, (data: T) => {
     handler(data);
    });
  }

  deregister(methodName: string){
    this.hubConnection.off(methodName);
    this.hubConnection.stop();
  }
}