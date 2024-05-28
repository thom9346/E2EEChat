import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Message } from '../models/Message';

@Injectable({
  providedIn: 'root'
})
export class ChatService {

  private apiUrl = 'https://localhost:5000/api'; 

  constructor(private http: HttpClient) {}

  private getHeaders() {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    console.log('Headers:', headers);
    return headers;
  }

  getMessages(): Observable<Message[]> {
    return this.http.get<Message[]>(`${this.apiUrl}/Messages`, { headers: this.getHeaders() });
  }

  getMessagesBetweenUsers(userId1: string, userId2: string): Observable<Message[]> {
    return this.http.get<Message[]>(`${this.apiUrl}/Messages/between/${userId1}/${userId2}`, { headers: this.getHeaders() });
  }

  sendMessage(message: Message): Observable<Message> {
    return this.http.post<Message>(`${this.apiUrl}/Messages`, message, { headers: this.getHeaders() });
  }
}
