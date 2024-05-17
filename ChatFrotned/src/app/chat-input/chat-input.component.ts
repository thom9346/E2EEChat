import { Component, Output, EventEmitter, Input } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { SignalRService } from '../services/signal-r.service';
import { DiffieHellmanService } from '../services/diffie-hellman.service';
import { EncryptionService } from '../services/encryption.service';  // Import EncryptionService
import { Message } from '../models/Message';
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

  constructor(
    private authService: AuthService,
    private signalRService: SignalRService,
    private diffieHellmanService: DiffieHellmanService,  // Inject DiffieHellmanService
    private encryptionService: EncryptionService  // Inject EncryptionService
  ) {}

  async sendMessage() {
    if (this.newMessage.trim() && this.recipient) {
      const currentUser = this.authService.getCurrentUser();

      const myPrivateKeyString = localStorage.getItem("privateKey");
      if (!myPrivateKeyString) {
        console.error('No private key found in local storage');
        return;
      }

      const myPrivateKey: JsonWebKey = JSON.parse(myPrivateKeyString);
      const otherPublicKey: JsonWebKey = JSON.parse(this.recipient.publicKey);

      if (!otherPublicKey) {
        console.error('No public key found for the recipient');
        return;
      }

      try {
        const secretArrayBuffer = await this.diffieHellmanService.computeSharedSecret(myPrivateKey, otherPublicKey);
        const secretKey = bufferToHex(secretArrayBuffer);

        const encryptedMessage = await this.encryptionService.encryptData(this.newMessage, secretKey);

        if (encryptedMessage) {
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
        }
      } catch (error) {
        console.error('Failed to compute shared secret or encrypt message:', error);
      }
    }
  }
}

function bufferToHex(buffer: ArrayBuffer): string {
  return Array.from(new Uint8Array(buffer))
    .map(b => b.toString(16).padStart(2, '0'))
    .join('');
}
