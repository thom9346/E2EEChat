export class Message {
    messageId?: string;
    content: string = '';
    timestamp: Date = new Date();
    senderId: string = '';
    recipientId: string = '';
}