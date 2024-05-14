import { Component, Output, EventEmitter } from '@angular/core';
import { ChatService } from '../services/chat.service';
import { Message } from '../models/Message';
import { AuthService } from '../services/auth.service';
@Component({
  selector: 'app-chat-input',
  templateUrl: './chat-input.component.html',
  styleUrls: ['./chat-input.component.scss']
})
export class ChatInputComponent {

  newMessage: string = '';
  @Output() messageSent = new EventEmitter<Message>();

  constructor(private chatService: ChatService, private authService: AuthService) {}

  sendMessage() {
    if (this.newMessage.trim()) {
      const currentUser = this.authService.getCurrentUser();
      console.log(currentUser)

      const messageToSend: Message = {
        content: this.newMessage,
        timestamp: new Date(),
        senderId: currentUser.nameid,
        recipientId: '3FA85F64-5717-4562-B3FC-2C963F66AFA7'
      };

      this.chatService.sendMessage(messageToSend).subscribe({
        next: (message) => {
          this.messageSent.emit(message);
          this.newMessage = '';
        },
        error: (error) => console.error('Failed to send message:', error)
      });
    }
  }
}
