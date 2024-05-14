import { Component, Output, EventEmitter, Input } from '@angular/core';
import { ChatService } from '../services/chat.service';
import { Message } from '../models/Message';
import { AuthService } from '../services/auth.service';
import { User } from '../models/User';
@Component({
  selector: 'app-chat-input',
  templateUrl: './chat-input.component.html',
  styleUrls: ['./chat-input.component.scss']
})
export class ChatInputComponent {

  @Input() recipient: User | null = null;
  newMessage: string = '';
  @Output() messageSent = new EventEmitter<Message>();

  constructor(private chatService: ChatService, private authService: AuthService) {}

  sendMessage() {
    if (this.newMessage.trim() && this.recipient) {
      const currentUser = this.authService.getCurrentUser();

      const messageToSend: Message = {
        content: this.newMessage,
        timestamp: new Date(),
        senderId: currentUser.nameid,
        recipientId: this.recipient.userId
      };
      console.log(currentUser)
      console.log("Here is recipient:")
      console.log(this.recipient)

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
