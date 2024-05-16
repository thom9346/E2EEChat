import { Injectable } from '@angular/core';
import * as crypto from 'crypto-js';

@Injectable({
  providedIn: 'root'
})
export class EncryptionService {
  private dh: crypto.lib.WordArray;
  private sharedSecret: string | null = null;

  constructor() {
    this.dh = crypto.lib.WordArray.random(128 / 8); // Generate private key
  }

  // Generate public key
  getPublicKey(): string {
    return this.dh.toString(crypto.enc.Hex);
  }

  // Compute the shared secret
  computeSharedSecret(otherPublicKey: string): void {
    const otherKey = crypto.enc.Hex.parse(otherPublicKey);
    this.sharedSecret = crypto.HmacSHA256(otherKey, this.dh).toString();
  }

  // Encrypt message
  encryptMessage(message: string): string {
    if (!this.sharedSecret) {
      throw new Error('Shared secret is not set. Cannot encrypt message.');
    }
    return crypto.AES.encrypt(message, this.sharedSecret).toString();
  }

  // Decrypt message
  decryptMessage(encryptedMessage: string): string {
    if (!this.sharedSecret) {
      throw new Error('Shared secret is not set. Cannot decrypt message.');
    }
    const bytes = crypto.AES.decrypt(encryptedMessage, this.sharedSecret);
    return bytes.toString(crypto.enc.Utf8);
  }
}
