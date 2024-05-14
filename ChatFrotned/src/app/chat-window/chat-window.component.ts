import { Component, OnInit } from '@angular/core';
import { ChatService } from '../services/chat.service';
import { Message } from '../models/Message';

@Component({
  selector: 'app-chat-window',
  templateUrl: './chat-window.component.html',
  styleUrls: ['./chat-window.component.scss']
})
export class ChatWindowComponent implements OnInit {

  messages: Message[] = [];

  constructor(private chatService: ChatService) {}

  ngOnInit() {
    this.loadMessages();
  }

  loadMessages() {
    this.chatService.getMessages().subscribe({
      next: (data) => this.messages = data,
      error: (error) => console.error('Failed to get messages:', error)
    });
  }

  onMessageSent(message: Message) {
    this.messages.push(message);
  }
}
