import { Component, OnInit, AfterViewChecked, ElementRef, ViewChild } from '@angular/core';
import { ChatService } from '../services/chat.service';
import { Message } from '../models/Message';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';
import { User } from '../models/User';
import { SignalRService } from '../services/signal-r.service';
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
  recipientPublicKeyReceived = false;

  @ViewChild('messagesContainer') private messagesContainer!: ElementRef;

  constructor(private chatService: ChatService, private authService: AuthService, private router: Router, private signalRService: SignalRService, private encryptionService: EncryptionService) {
    this.currentUser = this.authService.getCurrentUser();
  }

  ngOnInit() {
    this.signalRService.messageReceived$.subscribe((message: Message) => {
      if (this.selectedRecipient && (message.senderId === this.selectedRecipient.userId || message.recipientId === this.selectedRecipient.userId)) {
        message.content = this.encryptionService.decryptMessage(message.content);
        this.messages.push(message);
        this.scrollToBottom();
      }
    });

    this.signalRService.publicKeyReceived$.subscribe((publicKey: string) => {
      if (!this.recipientPublicKeyReceived) {
        this.encryptionService.computeSharedSecret(publicKey);
        const publicKeyToSend = this.encryptionService.getPublicKey();
        this.signalRService.sendPublicKey(publicKeyToSend, this.selectedRecipient!.connectionId).catch(error => console.error('Failed to send public key:', error));
        this.recipientPublicKeyReceived = true;
      } else {
        this.encryptionService.computeSharedSecret(publicKey);
      }
    });
  }

  ngAfterViewChecked() {
    this.scrollToBottom();
  }

  loadMessages() {
    if (this.selectedRecipient) {
      this.chatService.getMessagesBetweenUsers(this.currentUser.nameid, this.selectedRecipient.userId).subscribe({
        next: (data) => {
          this.messages = this.chatService.decryptMessages(data),
          this.scrollToBottom();
        }, 
        error: (error) => console.error('Failed to get messages:', error)
      });
    }
  }

  onUserSelected(user: User) {
    this.selectedRecipient = user;
    this.loadMessages();
    console.log(this.selectedRecipient);
  
    // Ensure the connection ID is present
    if (!this.selectedRecipient.connectionId) {
      console.error('No connection ID found for selected recipient');
      return;
    }
  
    // Send public key to the selected recipient
    const publicKey = this.encryptionService.getPublicKey();
    this.signalRService.sendPublicKey(publicKey, this.selectedRecipient.connectionId)
      .catch(error => console.error('Failed to send public key:', error));
  }

  logout() {
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
}
