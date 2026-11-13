import { Component, ElementRef, effect, inject, signal, viewChild } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ChatSignalRService } from './services/chat-signalr.service';

@Component({
  selector: 'app-root',
  imports: [FormsModule, DatePipe],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  protected readonly chat = inject(ChatSignalRService);

  protected readonly username = signal('');
  protected readonly draft = signal('');
  protected readonly joined = signal(false);
  protected readonly error = signal<string | null>(null);

  private readonly messageList = viewChild<ElementRef<HTMLElement>>('messageList');

  constructor() {
    // Gulir otomatis ke pesan terbaru setiap ada pesan masuk
    effect(() => {
      this.chat.messages();
      const el = this.messageList()?.nativeElement;
      if (el) {
        queueMicrotask(() => (el.scrollTop = el.scrollHeight));
      }
    });
  }

  protected async join(): Promise<void> {
    const name = this.username().trim();
    if (!name) {
      return;
    }

    this.error.set(null);
    try {
      await this.chat.start(name);
      this.joined.set(true);
    } catch {
      this.error.set('Gagal terhubung ke server. Pastikan backend berjalan di http://localhost:5000.');
    }
  }

  protected async send(): Promise<void> {
    const text = this.draft().trim();
    if (!text) {
      return;
    }

    await this.chat.sendMessage(text);
    this.draft.set('');
  }

  protected async leave(): Promise<void> {
    await this.chat.stop();
    this.joined.set(false);
  }
}
