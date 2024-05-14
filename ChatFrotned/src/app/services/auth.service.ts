import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  // Mock user data, in real application this would come from a backend API or authentication service
  private currentUser = {
    id: '3FA85F64-5717-4562-B3FC-2C963F66AFA6', // Example user ID
    name: 'John Doe'
  };

  getCurrentUser() {
    return this.currentUser;
  }
  
}
