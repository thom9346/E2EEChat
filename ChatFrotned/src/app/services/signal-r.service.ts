import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Message } from '../models/Message';
import { Subject } from 'rxjs';
import { ChatService } from './chat.service';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {

  private hubConnection: signalR.HubConnection;
  private messageReceived = new Subject<Message>();

  messageReceived$ = this.messageReceived.asObservable();

  constructor(private chatService: ChatService) {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:5000/chathub')
      .build();

    this.hubConnection.on('ReceiveMessage', (message: Message) => {
      this.messageReceived.next(message);
    });

    this.hubConnection.start().catch(err => console.error('Error while starting SignalR connection: ' + err));
  }

  sendMessage(message: Message) {
    return this.chatService.sendMessage(message).toPromise();
  }
}
