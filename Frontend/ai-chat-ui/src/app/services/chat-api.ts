
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ChatRequest {
  message: string;
  conversationId?: string | null;
}

export interface ChatReply {
  conversationId: string;
  response: string;
}

@Injectable({
  providedIn: 'root'
})
export class ChatApiService {
  private readonly http = inject(HttpClient);

  sendMessage(request: ChatRequest): Observable<ChatReply> {
    return this.http.post<ChatReply>('/api/chat', request);
  }
}
