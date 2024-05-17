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
  private currentGroup: string | null = null;

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

  joinGroup(groupName: string) {
    if (this.currentGroup) {
      this.leaveGroup(this.currentGroup);
    }
    this.hubConnection.invoke('JoinGroup', groupName).then(() => {
      this.currentGroup = groupName;
    }).catch(err => console.error('Error while joining group: ' + err));
  }

  leaveGroup(groupName: string) {
    this.hubConnection.invoke('LeaveGroup', groupName).then(() => {
      this.currentGroup = null;
    }).catch(err => console.error('Error while leaving group: ' + err));
  }

  async sendMessage(message: Message, groupName: string): Promise<void> {
    try {
      const savedMessage = await this.chatService.sendMessage(message).toPromise();
      await this.hubConnection.invoke('SendMessage', groupName, savedMessage);
    } catch (err) {
      console.error('Error while sending message:', err);
    }
  }
}
