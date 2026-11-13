import { Injectable, signal } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { ChatMessage, ConnectionStatus } from '../models/chat-message';

const HUB_URL = 'http://localhost:5000/chathub';

// Slide 10 & 13: satu service = satu koneksi hub untuk seluruh aplikasi
@Injectable({ providedIn: 'root' })
export class ChatSignalRService {
  private connection: signalR.HubConnection | null = null;
  private username = '';

  readonly messages = signal<ChatMessage[]>([]);
  readonly onlineUsers = signal<string[]>([]);
  readonly status = signal<ConnectionStatus>('disconnected');

  async start(username: string): Promise<void> {
    // Praktik terbaik (slide 13): jangan membuat koneksi berulang kali
    if (this.connection) {
      return;
    }

    this.username = username;

    const connection = new signalR.HubConnectionBuilder()
      .withUrl(HUB_URL)
      .withAutomaticReconnect() // Slide 10: reconnect otomatis
      .build();

    // Listener event dari server (nama event harus sama dengan di C#)
    connection.on('ReceiveMessage', (msg: ChatMessage) =>
      this.messages.update((list) => [...list, msg])
    );
    connection.on('OnlineUsers', (users: string[]) => this.onlineUsers.set(users));

    connection.onreconnecting(() => this.status.set('reconnecting'));

    // Koneksi baru berarti ConnectionId baru di server, sehingga nama harus didaftarkan ulang
    connection.onreconnected(async () => {
      this.status.set('connected');
      await connection.invoke('JoinChat', this.username);
    });

    connection.onclose(() => {
      this.status.set('disconnected');
      this.connection = null;
    });

    this.connection = connection;
    this.status.set('connecting');

    try {
      await connection.start();
      this.status.set('connected');
      await connection.invoke('JoinChat', username);
    } catch (err) {
      this.status.set('disconnected');
      this.connection = null;
      throw err;
    }
  }

  async sendMessage(text: string): Promise<void> {
    await this.connection?.invoke('SendMessage', text);
  }

  async stop(): Promise<void> {
    await this.connection?.stop();
    this.connection = null;
    this.messages.set([]);
    this.onlineUsers.set([]);
    this.status.set('disconnected');
  }
}
