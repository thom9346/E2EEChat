import { Injectable } from '@angular/core';
import * as crypto from 'crypto';
@Injectable({
  providedIn: 'root'
})
export class EncryptionService {

  generateKeyPair(): { publicKey: string, privateKey: string } {
    const ecdh = crypto.createECDH('secp256k1');
    const publicKey = ecdh.generateKeys('hex', 'compressed');
    const privateKey = ecdh.getPrivateKey('hex');
    return { publicKey, privateKey };
  }

  computeSharedSecret(privateKey: string, otherPublicKey: string): string {
    const ecdh = crypto.createECDH('secp256k1');
    ecdh.setPrivateKey(privateKey, 'hex');
    const sharedSecret = ecdh.computeSecret(otherPublicKey, 'hex');
    // Hash the shared secret to produce a 256-bit key
    return crypto.createHash('sha256').update(sharedSecret).digest('hex');
  }

  encryptData(data: string, encryptionKey: string): string {
    const iv = crypto.randomBytes(16);
    const cipher = crypto.createCipheriv('aes-256-cbc', Buffer.from(encryptionKey, 'hex'), iv);
    let encrypted = cipher.update(data, 'utf8', 'hex');
    encrypted += cipher.final('hex');
    return iv.toString('hex') + encrypted;
  }

  decryptData(encrypted: string, encryptionKey: string): string {
    const iv = Buffer.from(encrypted.slice(0, 32), 'hex');
    const encryptedText = encrypted.slice(32);
    const decipher = crypto.createDecipheriv('aes-256-cbc', Buffer.from(encryptionKey, 'hex'), iv);
    let decrypted = decipher.update(encryptedText, 'hex', 'utf8');
    decrypted += decipher.final('utf8');
    return decrypted;
  }
}
