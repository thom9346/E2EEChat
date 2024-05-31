import { Component, Output, EventEmitter, Input } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { SignalRService } from '../services/signal-r.service';
import { DiffieHellmanService } from '../services/diffie-hellman.service';
import { EncryptionService } from '../services/encryption.service';
import { Message } from '../models/Message';
import { User } from '../models/User';
import { RsaService } from '../services/rsa.service';

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
    private diffieHellmanService: DiffieHellmanService,
    private encryptionService: EncryptionService,
    private rsaService: RsaService
  ) {}

  async sendMessage() {
   
    if (this.newMessage.trim() && this.recipient) {
      const currentUser = this.authService.getCurrentUser();
      const privateKeyString = localStorage.getItem("privateKey");
      const privateSigningKeyString = localStorage.getItem("privateSigningKey");

      if (!privateKeyString || !privateSigningKeyString) {
        console.error('No private key or signing key found in local storage');
        return;
      }

      const myPrivateKey: JsonWebKey = JSON.parse(privateKeyString);
      const otherPublicKey: JsonWebKey = JSON.parse(this.recipient.publicKey);

      if (!otherPublicKey) {
        console.error('No public key found for the recipient');
        return;
      }


      try {
        const secretArrayBuffer = await this.diffieHellmanService.computeSharedSecret(myPrivateKey, otherPublicKey);
        const secretKey = bufferToHex(secretArrayBuffer);

        const signature = await this.rsaService.signData(this.newMessage, privateSigningKeyString);
        const encryptedMessage = await this.encryptionService.encryptData(this.newMessage, secretKey);

        if (encryptedMessage) {
          const messageToSend: Message = {
            content: encryptedMessage,
            signature: signature,
            timestamp: new Date(),
            senderId: currentUser.userId,
            recipientId: this.recipient.userId
          };

          const groupName = this.getGroupName(currentUser.userId, this.recipient.userId);
          this.signalRService.sendMessage(messageToSend, groupName).then(() => {
            this.messageSent.emit(messageToSend);
            this.newMessage = '';
          }).catch(error => console.error('Failed to send message:', error));
        }
      } catch (error) {
        console.error('Failed to compute shared secret or encrypt message:', error);
      }
    }
  }

  private getGroupName(userId1: string, userId2: string): string {
    return [userId1, userId2].sort().join('_');
  }
}

function bufferToHex(buffer: ArrayBuffer): string {
  return Array.from(new Uint8Array(buffer))
    .map(b => b.toString(16).padStart(2, '0'))
    .join('');
}
