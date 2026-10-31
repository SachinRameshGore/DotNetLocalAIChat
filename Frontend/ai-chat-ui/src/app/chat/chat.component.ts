import { CommonModule } from '@angular/common';
import {  Component,  ChangeDetectorRef,   ElementRef,   ViewChild,   inject} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs';
import { ChatApiService } from '../services/chat-api';

interface ChatMessage {
  role: 'user' | 'assistant';
  content: string;
}

@Component({

  selector: 'app-chat',
  styleUrl: './chat.component.css',
  templateUrl: './chat.component.html',
  standalone: true,
  imports: [CommonModule, FormsModule],
})

export class ChatComponent {

  private readonly chatApi = inject(ChatApiService);
  private readonly cdr = inject(ChangeDetectorRef);
  
  @ViewChild('messageList')
  private messageList?: ElementRef<HTMLDivElement>;

  messages: ChatMessage[] = [];
  draft = '';
  conversationId: string | null = null;
  loading = false;
  error = '';

 
sendMessage(): void {
  const message = this.draft.trim();

  if (!message || this.loading) {
    return;
  }

  this.error = '';
  this.draft = '';

  this.messages = [
    ...this.messages,
    { role: 'user', content: message }
  ];

  this.loading = true;
  this.scrollToBottom();

  this.chatApi.sendMessage({
    message,
    conversationId: this.conversationId
  })
  .pipe(
    finalize(() => {
      this.loading = false;
      this.cdr.detectChanges();
      this.scrollToBottom();
    })
  )
  .subscribe({
    next: (reply) => {
      
      this.conversationId = reply.conversationId;

      this.messages = [
        ...this.messages,
        {
          role: 'assistant',
          content: reply.response
        }
      ];

      this.cdr.detectChanges();
      this.scrollToBottom();
    },
    error: (err) => {
      console.error('Chat request failed:', err);

      this.error = err.status === 0
        ? 'Cannot connect to the API. Check that .NET is running.'
        : err.status === 503
          ? 'The AI service is unavailable. Check Ollama.'
          : err.status === 504
            ? 'The AI response timed out. Please try again.'
            : 'Failed to get an AI response. Please try again.';

      this.cdr.detectChanges();
    }
  });
}
  startNewChat(): void {
    if (this.loading) {
      return;
    }

    this.messages = [];
    this.conversationId = null;
    this.draft = '';
    this.error = '';
  }
  onEnter(event: Event): void {
    const keyboardEvent = event as KeyboardEvent;

    if (keyboardEvent.key === 'Enter' && !keyboardEvent.shiftKey) {
      keyboardEvent.preventDefault();
      this.sendMessage();
    }
  }

  private scrollToBottom(): void {
    setTimeout(() => {
      const element = this.messageList?.nativeElement;

      if (element) {
        element.scrollTop = element.scrollHeight;
      }
    });
  }

}
