import { Component, Output, EventEmitter, Input } from '@angular/core';
import { ChatService } from '../services/chat.service';
import { Message } from '../models/Message';
import { AuthService } from '../services/auth.service';
import { User } from '../models/User';
import { SignalRService } from '../services/signal-r.service';
import { EncryptionService } from '../services/encryption.service';

@Component({
  selector: 'app-chat-input',
  templateUrl: './chat-input.component.html',
  styleUrls: ['./chat-input.component.scss']
})
export class ChatInputComponent {

  @Input() recipient: User | null = null;
  newMessage: string = '';
  @Output() messageSent = new EventEmitter<Message>();

  constructor(private authService: AuthService, private signalRService: SignalRService, private encryptionService: EncryptionService) {}

  sendMessage() {
    if (this.newMessage.trim() && this.recipient) {
      const currentUser = this.authService.getCurrentUser();

      try {
        // Encrypt the message before sending
        const encryptedMessage = this.encryptionService.encryptMessage(this.newMessage);

        const messageToSend: Message = {
          content: encryptedMessage,
          timestamp: new Date(),
          senderId: currentUser.nameid,
          recipientId: this.recipient.userId
        };

        this.signalRService.sendMessage(messageToSend).then(() => {
          this.messageSent.emit(messageToSend);
          this.newMessage = '';
        }).catch(error => console.error('Failed to send message:', error));
      } catch (error) {
        console.error('Failed to encrypt message:', error);
      }
    }
  }
}
