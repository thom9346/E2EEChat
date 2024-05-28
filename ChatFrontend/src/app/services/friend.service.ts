import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class FriendService {

  private apiUrl = 'https://localhost:5000/api';

  constructor(private http: HttpClient) {}

  sendFriendRequest(requesterId: string, requesteeEmail: string, requesterSigningPublicKey: string): Observable<any> {
    const headers = new HttpHeaders().set('Content-Type', 'application/json');
    const body = { requesterId, requesteeEmail, requesterSigningPublicKey };
    return this.http.post<any>(`${this.apiUrl}/Friendship/send-friend-request`, body, { headers });
  }

  confirmFriendRequest(requestId: string, token: string, requesteePublicSigningKey: string, requesteeId: string): Observable<any> {
    const headers = new HttpHeaders().set('Content-Type', 'application/json');
    const body = { requestId, token, requesteePublicSigningKey, requesteeId };
    return this.http.post<any>(`${this.apiUrl}/Friendship/confirm-friend-request`, body, { headers });
  }

  checkFriendRequestStatus(requesterId: string, requesteeId: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/Friendship/check-friend-request-status/${requesterId}/${requesteeId}`);
  }
}
