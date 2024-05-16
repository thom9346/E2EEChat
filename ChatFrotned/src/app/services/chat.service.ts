import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Message } from '../models/Message';
import { EncryptionService } from './encryption.service';

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private apiUrl = 'https://localhost:5000/api'; 

  constructor(private http: HttpClient, private encryptionService: EncryptionService) {}

  getMessages(): Observable<Message[]> {
    return this.http.get<Message[]>(`${this.apiUrl}/Messages`);
  }

  getMessagesBetweenUsers(userId1: string, userId2: string): Observable<Message[]> {
    return this.http.get<Message[]>(`${this.apiUrl}/Messages/between/${userId1}/${userId2}`);
  }

  sendMessage(message: Message): Observable<Message> {
    message.content = this.encryptionService.encryptMessage(message.content);
    return this.http.post<Message>(`${this.apiUrl}/Messages`, message);
  }

  decryptMessages(messages: Message[]): Message[] {
    return messages.map(message => {
      message.content = this.encryptionService.decryptMessage(message.content);
      return message;
    });
  }
}
