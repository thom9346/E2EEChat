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
  // async generateSigningKeyPair(): Promise<{ publicKey: string, privateKey: string }> {
  //   const keyPair = await crypto.subtle.generateKey(
  //     {
  //       name: 'ECDSA',
  //       namedCurve: 'P-256'
  //     },
  //     true, // extractable
  //     ['sign', 'verify']
  //   );
  
  //   const publicKey = await crypto.subtle.exportKey('spki', keyPair.publicKey);
  //   const privateKey = await crypto.subtle.exportKey('pkcs8', keyPair.privateKey);
  
  //   const publicKeyBase64 = btoa(String.fromCharCode(...new Uint8Array(publicKey))); 
  //   const privateKeyBase64 = btoa(String.fromCharCode(...new Uint8Array(privateKey))); 

  //   return { publicKey: publicKeyBase64, privateKey: privateKeyBase64 }; 
  // }
  // async signData(data: string, privateKeyBase64: string): Promise<string> { 
  //   const privateKeyArrayBuffer = Uint8Array.from(atob(privateKeyBase64), c => c.charCodeAt(0)).buffer; 
  //   const privateKey = await crypto.subtle.importKey(
  //     'pkcs8',
  //     privateKeyArrayBuffer,
  //     {
  //       name: 'ECDSA',
  //       namedCurve: 'P-256'
  //     },
  //     false,
  //     ['sign']
  //   );

  //   const encoder = new TextEncoder();
  //   const encodedData = encoder.encode(data);

  //   const signature = await crypto.subtle.sign(
  //     {
  //       name: 'ECDSA',
  //       hash: { name: 'SHA-256' }
  //     },
  //     privateKey,
  //     encodedData
  //   );

  //   return btoa(String.fromCharCode(...new Uint8Array(signature))); 
  // }

  
}
