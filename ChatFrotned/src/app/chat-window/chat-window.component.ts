import { Component, OnInit } from '@angular/core';
import { ChatService } from '../services/chat.service';
import { Message } from '../models/Message';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';
import { User } from '../models/User';

@Component({
  selector: 'app-chat-window',
  templateUrl: './chat-window.component.html',
  styleUrls: ['./chat-window.component.scss']
})
export class ChatWindowComponent implements OnInit {

  messages: Message[] = [];
  selectedRecipient: User | null = null;
  currentUser: any;

  constructor(private chatService: ChatService, private authService: AuthService, private router: Router) {
    this.currentUser = this.authService.getCurrentUser();
  }

  ngOnInit() {
    //this.loadMessages();
  }

  loadMessages() {
    if (this.selectedRecipient) {
      console.log(this.selectedRecipient)
      console.log("user current")
      console.log(this.currentUser)
      this.chatService.getMessagesBetweenUsers(this.currentUser.nameid, this.selectedRecipient.userId).subscribe({
        next: (data) => this.messages = data,
        error: (error) => console.error('Failed to get messages:', error)
      });
    }
  }

  onMessageSent(message: Message) {
    this.messages.push(message);
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
