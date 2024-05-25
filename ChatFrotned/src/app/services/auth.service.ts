import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { DiffieHellmanService } from './diffie-hellman.service';
import { RsaService } from './rsa.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'https://localhost:5000/api'; // Adjust the URL as needed
  private currentUserSubject = new BehaviorSubject<any>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(
    private http: HttpClient,
    private diffieHellmanService: DiffieHellmanService,
    private rsaService: RsaService) {
    const token = localStorage.getItem('token');
    const userId = localStorage.getItem('userId');
    if (token && userId) {
      this.currentUserSubject.next({ token, userId });
    }
  }

  login(username: string, password: string): Observable<any> {
    const privateKey = localStorage.getItem('privateSigningKey'); //CHANGE: Removed JSON.parse

    if (!privateKey) {
      throw new Error('No private signing key found in local storage');
    }
    return new Observable(observer => {

        this.http.post<any>(`${this.apiUrl}/Auth/Login`, { username, password }).pipe(
          tap(response => {
            localStorage.setItem('token', response.jwt);
            localStorage.setItem('userId', response.userId.toString());
            this.currentUserSubject.next({ token: response.jwt, userId: response.userId });
          })
        ).subscribe({
          next: (response) => {
            observer.next(response);
            observer.complete();
          },
          error: (error) => {
            observer.error(error);
          }
        });

    });
  }

  async register(username: string, email: string, password: string): Promise<Observable<any>> {
    const { publicKey: dhPublicKey, privateKey: dhPrivateKey } = await this.diffieHellmanService.generateECDHKeyPair();
    const { publicKey: signingPublicKey, privateKey: signingPrivateKey } = await this.rsaService.generateRSASigningKeyPair();
    //const { publicKey: signingPublicKey, privateKey: signingPrivateKey } = await this.diffieHellmanService.generateSigningKeyPair();

    localStorage.setItem('privateKey', JSON.stringify(dhPrivateKey));
    localStorage.setItem('privateSigningKey', signingPrivateKey);
    localStorage.setItem('publicSigningKey', signingPublicKey);

    const publicKeyJson = JSON.stringify(dhPublicKey);

    const registrationData = {
      username,
      email,
      password,
      publicKey: publicKeyJson,
      signingPublicKey: signingPublicKey
    };

    return this.http.post<any>(`${this.apiUrl}/Auth/Register`, registrationData);
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('userId');
    localStorage.removeItem('privateSigningKey');
    this.currentUserSubject.next(null);
  }

  getCurrentUser() {
    return this.currentUserSubject.value;
  }
}
