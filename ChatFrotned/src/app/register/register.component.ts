import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { EncryptionService } from '../services/encryption.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {
  username: string = '';
  email: string = '';
  password: string = '';
  confirmPassword: string = '';

  constructor(private authService: AuthService, private router: Router, private encryptionService: EncryptionService) {}

  onSubmit() {
    if (this.password !== this.confirmPassword) {
      console.error('Passwords do not match');
      return;
    }
    const { publicKey, privateKey } = this.encryptionService.generateKeyPair();
    localStorage.setItem('privateKey', privateKey); // Store private key locally

    this.authService.register(this.username, this.email, this.password, publicKey).subscribe({
      next: () => this.router.navigate(['/login']),
      error: (error) => console.error('Failed to register:', error)
    });
  }
}
