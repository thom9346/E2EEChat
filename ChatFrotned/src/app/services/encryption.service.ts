import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class EncryptionService {

  constructor() { }

  private async hashKey(key: string): Promise<ArrayBuffer> {
    const encodedKey = new TextEncoder().encode(key);
    return crypto.subtle.digest('SHA-256', encodedKey);
  }

  async encryptData(data: any, encryptionKey: string): Promise<string | null> {
    try {
      const hashedKeyBuffer = await this.hashKey(encryptionKey);
      const key = await crypto.subtle.importKey(
        'raw',
        hashedKeyBuffer,
        'AES-GCM',
        false,
        ['encrypt']
      );

      const iv = crypto.getRandomValues(new Uint8Array(12));
      const encodedData = new TextEncoder().encode(JSON.stringify(data));
      const ciphertext = await crypto.subtle.encrypt(
        { name: 'AES-GCM', iv: iv },
        key,
        encodedData
      );

      const ivStr = Array.from(iv).map(b => String.fromCharCode(b)).join('');
      const encryptedDataStr = Array.from(new Uint8Array(ciphertext)).map(b => String.fromCharCode(b)).join('');
      return btoa(ivStr + encryptedDataStr);
    } catch (error) {
      console.error('Encryption error:', error);
      return null;
    }
  }

  async decryptData(ciphertext: string, encryptionKey: string): Promise<any> {
    try {
      const hashedKeyBuffer = await this.hashKey(encryptionKey);
      const key = await crypto.subtle.importKey(
        'raw',
        hashedKeyBuffer,
        'AES-GCM',
        false,
        ['decrypt']
      );

      const ciphertextBytes = Uint8Array.from(atob(ciphertext), c => c.charCodeAt(0));
      const iv = ciphertextBytes.slice(0, 12);
      const encryptedData = ciphertextBytes.slice(12);

      const decryptedData = await crypto.subtle.decrypt(
        { name: 'AES-GCM', iv: iv },
        key,
        encryptedData
      );

      return JSON.parse(new TextDecoder().decode(decryptedData));
    } catch (error) {
      console.error('Decryption error:', error);
      return null;
    }
  }
}
