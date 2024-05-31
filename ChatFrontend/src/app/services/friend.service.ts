import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class FriendService {

  private apiUrl = 'https://localhost:5000/api';

  constructor(private http: HttpClient) {}

  private getHeaders() {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return headers;
  }


  sendFriendRequest(requesterId: string, requesteeEmail: string, requesterSigningPublicKey: string): Observable<any> {
    
    const body = { requesterId, requesteeEmail, requesterSigningPublicKey };
    return this.http.post<any>(`${this.apiUrl}/Friendship/send-friend-request`, body, { headers: this.getHeaders() });
  }

  confirmFriendRequest(requestId: string, token: string, requesteePublicSigningKey: string, requesteeId: string): Observable<any> {
    const body = { requestId, token, requesteePublicSigningKey, requesteeId };
    return this.http.post<any>(`${this.apiUrl}/Friendship/confirm-friend-request`, body, { headers: this.getHeaders() });
  }

  checkFriendRequestStatus(requesterId: string, requesteeId: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/Friendship/check-friend-request-status/${requesterId}/${requesteeId}`,  { headers: this.getHeaders() });
  }
}
