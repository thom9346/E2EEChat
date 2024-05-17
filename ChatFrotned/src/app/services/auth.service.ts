import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { DiffieHellmanService } from './diffie-hellman.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'https://localhost:5000/api'; // Adjust the URL as needed
  private currentUserSubject = new BehaviorSubject<any>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient, private diffieHellmanService: DiffieHellmanService) {
    const token = localStorage.getItem('token');
    if (token) {
      this.currentUserSubject.next(this.decodeToken(token));
    }
  }

  login(username: string, password: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/Auth/Login`, { username, password }).pipe(
      tap(response => {
        localStorage.setItem('token', response.token);
        this.currentUserSubject.next(this.decodeToken(response.token));
      })
    );
  }

  async register(username: string, email: string, password: string): Promise<Observable<any>> {

    const { publicKey, privateKey } = await this.diffieHellmanService.generateECDHKeyPair();

    localStorage.setItem('privateKey', JSON.stringify(privateKey));
    const publicKeyJson = JSON.stringify(publicKey);

    const registrationData = {
      username,
      email,
      password,
      publicKey: publicKeyJson  
    };

      return this.http.post<any>(`${this.apiUrl}/Auth/Register`, registrationData);
  }

  logout() {
    localStorage.removeItem('token');
    this.currentUserSubject.next(null);
  }

  getCurrentUser() {
    return this.currentUserSubject.value;
  }

  private decodeToken(token: string) {
    // Simple JWT token decoding (not secure, for demonstration only)
    const payload = atob(token.split('.')[1]);
    return JSON.parse(payload);
  }
}
