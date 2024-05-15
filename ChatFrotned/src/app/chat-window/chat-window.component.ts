import { Component, OnInit } from '@angular/core';
import { ChatService } from '../services/chat.service';
import { Message } from '../models/Message';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';
import { User } from '../models/User';
import { SignalRService } from '../services/signal-r.service';

@Component({
  selector: 'app-chat-window',
  templateUrl: './chat-window.component.html',
  styleUrls: ['./chat-window.component.scss']
})
export class ChatWindowComponent implements OnInit {

  messages: Message[] = [];
  selectedRecipient: User | null = null;
  currentUser: any;

  constructor(private chatService: ChatService, private authService: AuthService, private router: Router, private signalRService: SignalRService) {
    this.currentUser = this.authService.getCurrentUser();
  }

  ngOnInit() {
    this.signalRService.messageReceived$.subscribe((message: Message) => {
      if (this.selectedRecipient && (message.senderId === this.selectedRecipient.userId || message.recipientId === this.selectedRecipient.userId)) {
        this.messages.push(message);
      }
    });
  }

  loadMessages() {
    if (this.selectedRecipient) {
      this.chatService.getMessagesBetweenUsers(this.currentUser.nameid, this.selectedRecipient.userId).subscribe({
        next: (data) => this.messages = data,
        error: (error) => console.error('Failed to get messages:', error)
      });
    }
  }

  onUserSelected(user: User) {
    this.selectedRecipient = user;
    this.loadMessages();
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
