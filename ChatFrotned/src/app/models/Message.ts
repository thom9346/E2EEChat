export class Message {
    messageId?: string;
    content: string = '';
    signature: string = '';
    timestamp: Date = new Date();
    senderId: string = '';
    recipientId: string = '';
    signingPublicKey: string = '';
}