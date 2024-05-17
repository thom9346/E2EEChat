import { Component, OnInit, AfterViewChecked, ElementRef, ViewChild } from '@angular/core';
import { ChatService } from '../services/chat.service';
import { Message } from '../models/Message';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';
import { User } from '../models/User';
import { SignalRService } from '../services/signal-r.service';
import { DiffieHellmanService } from '../services/diffie-hellman.service';
import { EncryptionService } from '../services/encryption.service';

@Component({
  selector: 'app-chat-window',
  templateUrl: './chat-window.component.html',
  styleUrls: ['./chat-window.component.scss']
})
export class ChatWindowComponent implements OnInit, AfterViewChecked {

  messages: Message[] = [];
  selectedRecipient: User | null = null;
  currentUser: any;
  currentGroup: string | null = null;

  @ViewChild('messagesContainer') private messagesContainer!: ElementRef;

  constructor(
    private chatService: ChatService,
    private authService: AuthService, 
    private router: Router, 
    private signalRService: SignalRService,
    private diffieHellmanService: DiffieHellmanService,
    private encryptionService: EncryptionService
  ) {
    this.currentUser = this.authService.getCurrentUser();
  }

  ngOnInit() {
    this.signalRService.messageReceived$.subscribe((message: Message) => {
      if (this.shouldAttemptDecryption(message)) {
        if (this.isMessageForCurrentChat(message)) {
          this.decryptAndAddMessage(message);
          this.scrollToBottom();
        }
      }
    });
  }

  ngAfterViewChecked() {
    this.scrollToBottom();
  }

  async loadMessages() {
    const myPrivateKeyString = localStorage.getItem("privateKey");

    if (!myPrivateKeyString) {
      console.error('No private key found in local storage');
      return;
    }
    if (this.selectedRecipient == null) {
      return;
    }

    const myPrivateKey: JsonWebKey = JSON.parse(myPrivateKeyString);
    const otherPublicKey: JsonWebKey = JSON.parse(this.selectedRecipient.publicKey);

    if (!otherPublicKey) {
      console.error('No public key found for the selected recipient');
      return;
    }

    try {
      const secretArrayBuffer = await this.diffieHellmanService.computeSharedSecret(myPrivateKey, otherPublicKey);
      const secretKey = bufferToHex(secretArrayBuffer);

      if (this.selectedRecipient) {
        this.chatService.getMessagesBetweenUsers(this.currentUser.nameid, this.selectedRecipient.userId).subscribe({
          next: async (data) => {
            const decryptedMessages = await Promise.all(data.map(async (message) => {
              if (this.shouldAttemptDecryption(message)) {
                try {
                  message.content = await this.encryptionService.decryptData(message.content, secretKey);
                } catch (error) {
                  console.error('Failed to decrypt message:', error);
                }
              }
              return message;
            }));
            this.messages = decryptedMessages;
            this.scrollToBottom();
          },
          error: (error) => console.error('Failed to get messages:', error)
        });
      }
    } catch (error) {
      console.error('Failed to compute shared secret:', error);
    }
  }

  async decryptAndAddMessage(message: Message) {
    const myPrivateKeyString = localStorage.getItem("privateKey");

    if (!myPrivateKeyString) {
      console.error('No private key found in local storage');
      return;
    }
    if (this.selectedRecipient == null) {
      return;
    }

    const myPrivateKey: JsonWebKey = JSON.parse(myPrivateKeyString);
    const otherPublicKey: JsonWebKey = JSON.parse(this.selectedRecipient.publicKey);

    if (!otherPublicKey) {
      console.error('No public key found for the selected recipient');
      return;
    }

    try {
      const secretArrayBuffer = await this.diffieHellmanService.computeSharedSecret(myPrivateKey, otherPublicKey);
      const secretKey = bufferToHex(secretArrayBuffer);

      if (this.shouldAttemptDecryption(message)) {
        try {
          message.content = await this.encryptionService.decryptData(message.content, secretKey);
          if (!this.messages.some(m => m.timestamp === message.timestamp && m.senderId === message.senderId)) {
            this.messages.push(message);
            this.scrollToBottom();
          }
        } catch (error) {
          console.error('Failed to decrypt message:', error);
        }
      }
    } catch (error) {
      console.error('Failed to decrypt message:', error);
    }
  }

  shouldAttemptDecryption(message: Message): boolean {
    return message.senderId === this.currentUser.nameid || message.recipientId === this.currentUser.nameid;
  }

  isMessageForCurrentChat(message: Message): boolean {
    return (message.senderId === this.currentUser.nameid && message.recipientId === this.selectedRecipient?.userId) ||
           (message.senderId === this.selectedRecipient?.userId && message.recipientId === this.currentUser.nameid);
  }

  onUserSelected(user: User) {
    if (this.currentGroup) {
      this.signalRService.leaveGroup(this.currentGroup);
    }
    this.selectedRecipient = user;
    const groupName = this.getGroupName(this.currentUser.nameid, user.userId);
    this.currentGroup = groupName;
    this.signalRService.joinGroup(groupName);
    this.loadMessages();
  }

  logout() {
    if (this.currentGroup) {
      this.signalRService.leaveGroup(this.currentGroup);
    }
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  private scrollToBottom(): void {
    try {
      this.messagesContainer.nativeElement.scrollTop = this.messagesContainer.nativeElement.scrollHeight;
    } catch (err) {
      console.error('Could not scroll to bottom:', err);
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
