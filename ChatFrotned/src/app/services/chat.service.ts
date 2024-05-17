import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Message } from '../models/Message';


@Injectable({
  providedIn: 'root'
})
export class ChatService {

  private apiUrl = 'https://localhost:5000/api'; 

  constructor(private http: HttpClient) {}

  getMessages(): Observable<Message[]> {
    return this.http.get<Message[]>(`${this.apiUrl}/Messages`);
  }
  getMessagesBetweenUsers(userId1: string, userId2: string): Observable<Message[]> {
    return this.http.get<Message[]>(`${this.apiUrl}/Messages/between/${userId1}/${userId2}`);
  }

  sendMessage(message: Message): Observable<Message> {
    console.log("In here...")
    return this.http.post<Message>(`${this.apiUrl}/Messages`, message);
  }
}
