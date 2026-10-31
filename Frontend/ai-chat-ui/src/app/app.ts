import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ChatComponent } from './chat/chat.component';

@Component({
  imports: [ChatComponent],
  selector: 'app-root',
  styleUrl: './app.css',
  templateUrl: './app.html',
})
export class App {
  protected readonly title = signal('ai-chat-ui');
}
