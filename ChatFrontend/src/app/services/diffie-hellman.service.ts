import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class DiffieHellmanService {

  constructor() { }

  async generateECDHKeyPair(): Promise<{ publicKey: JsonWebKey, privateKey: JsonWebKey }> {
    const keyPair = await crypto.subtle.generateKey(
      {
        name: 'ECDH',
        namedCurve: 'P-256'
      },
      true, // extractable
      ['deriveKey', 'deriveBits']
    );

    const publicKey = await crypto.subtle.exportKey('jwk', keyPair.publicKey);
    const privateKey = await crypto.subtle.exportKey('jwk', keyPair.privateKey);
    return { publicKey, privateKey };
  }



  async computeSharedSecret(privateKeyJwk: JsonWebKey, otherPublicKey: JsonWebKey): Promise<ArrayBuffer> {
    const privateKey = await crypto.subtle.importKey(
      'jwk',
      privateKeyJwk,
      {
        name: 'ECDH',
        namedCurve: 'P-256'
      },
      false,
      ['deriveKey', 'deriveBits']
    );

    const importedOtherPublicKey = await crypto.subtle.importKey(
      'jwk',
      otherPublicKey,
      {
        name: 'ECDH',
        namedCurve: 'P-256'
      },
      false,
      []
    );

    const sharedSecret = await crypto.subtle.deriveBits(
      {
        name: 'ECDH',
        public: importedOtherPublicKey
      },
      privateKey,
      256
    );

    return sharedSecret;
  }
}
