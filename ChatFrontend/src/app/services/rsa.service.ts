import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class RsaService {

  constructor() { }

  async generateRSASigningKeyPair(): Promise<{ publicKey: string, privateKey: string }> {
    const keyPair = await crypto.subtle.generateKey(
      {
        name: 'RSASSA-PKCS1-v1_5',
        modulusLength: 2048,
        publicExponent: new Uint8Array([1, 0, 1]),
        hash: { name: 'SHA-256' }
      },
      true,
      ['sign', 'verify']
    );

    const publicKey = await crypto.subtle.exportKey('spki', keyPair.publicKey);
    const privateKey = await crypto.subtle.exportKey('pkcs8', keyPair.privateKey);

    const publicKeyBase64 = this.arrayBufferToBase64(publicKey);
    const privateKeyBase64 = this.arrayBufferToBase64(privateKey);

    return { publicKey: publicKeyBase64, privateKey: privateKeyBase64 };
  }

  async signData(data: string, privateKeyBase64: string): Promise<string> {
    const privateKeyArrayBuffer = this.base64ToArrayBuffer(privateKeyBase64);
    const privateKey = await crypto.subtle.importKey(
      'pkcs8',
      privateKeyArrayBuffer,
      {
        name: 'RSASSA-PKCS1-v1_5',
        hash: { name: 'SHA-256' }
      },
      false,
      ['sign']
    );

    const encoder = new TextEncoder();
    const encodedData = encoder.encode(data);

    const signature = await crypto.subtle.sign(
      'RSASSA-PKCS1-v1_5',
      privateKey,
      encodedData
    );

    return this.arrayBufferToBase64(signature);
  }

  async verifySignature(data: string, signatureBase64: string, publicKeyBase64: string): Promise<boolean> {
    const signatureArrayBuffer = this.base64ToArrayBuffer(signatureBase64);
    const publicKeyArrayBuffer = this.base64ToArrayBuffer(publicKeyBase64);

    const publicKey = await crypto.subtle.importKey(
      'spki',
      publicKeyArrayBuffer,
      {
        name: 'RSASSA-PKCS1-v1_5',
        hash: { name: 'SHA-256' }
      },
      false,
      ['verify']
    );

    const encoder = new TextEncoder();
    const encodedData = encoder.encode(data);

    return crypto.subtle.verify(
      'RSASSA-PKCS1-v1_5',
      publicKey,
      signatureArrayBuffer,
      encodedData
    );
  }

  private arrayBufferToBase64(buffer: ArrayBuffer): string {
    const bytes = new Uint8Array(buffer);
    let binary = '';
    for (let i = 0; i < bytes.length; i++) {
      binary += String.fromCharCode(bytes[i]);
    }
    return btoa(binary);
  }

  private base64ToArrayBuffer(base64: string): ArrayBuffer {
    const binaryString = atob(base64);
    const len = binaryString.length;
    const bytes = new Uint8Array(len);
    for (let i = 0; i < len; i++) {
      bytes[i] = binaryString.charCodeAt(i);
    }
    return bytes.buffer;
  }
}
