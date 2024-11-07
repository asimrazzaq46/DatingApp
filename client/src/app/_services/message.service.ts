import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { Message } from '../_models/Message.model';
import { PaginatedResult } from '../_models/Pagination.model';
import { Group } from '../_models/Group';
import {
  setPaginatedResponse,
  setPaginationHeaders,
} from './paginationHelpers';
import {
  HubConnection,
  HubConnectionBuilder,
  HubConnectionState,
} from '@microsoft/signalr';
import { User } from '../_models/User.model';
import { group } from '@angular/animations';

@Injectable({
  providedIn: 'root',
})
export class MessageService {
  baseUrl = environment.apiUrl;
  hubsUrl = environment.hubsUrl;

  private http = inject(HttpClient);
  hubsConnection?: HubConnection;

  paginatedResult = signal<PaginatedResult<Message[]> | null>(null);
  messagesThread = signal<Message[]>([]);

  createHubConnection(user: User, otherUsername: string) {
    this.hubsConnection = new HubConnectionBuilder()
      .withUrl(this.hubsUrl + 'message?user=' + otherUsername, {
        accessTokenFactory: () => user.token,
      })
      .withAutomaticReconnect()
      .build();

    this.hubsConnection.start().catch((error) => console.log(error));

    this.hubsConnection.on('ReceiveMessageThread', (messages) => {
      this.messagesThread.set(messages);
    });

    this.hubsConnection.on('NewMessage', (newMessage) => {
      this.messagesThread.update((preMessage) => [...preMessage, newMessage]);
    });

    this.hubsConnection.on('UpdatedGroup', (group: Group) => {
       if (group.connections.some((x) => x.username === otherUsername)) {
        this.messagesThread.update((messages) => {
          messages.forEach((msg) => {
            if (!msg.dateRead) {
              msg.dateRead = new Date(Date.now());
            }
          });
          return messages;
        });
      }
    });
  }

  stopHubConnection() {
    if (this.hubsConnection?.state === HubConnectionState.Connected) {
      this.hubsConnection.stop().catch((error) => console.log(error));
    }
  }

  getMessages(pageNumber: number, pageSize: number, container: string) {
    let params = setPaginationHeaders(pageNumber, pageSize);
    params = params.append('Container', container);

    return this.http
      .get<Message[]>(`${this.baseUrl}messages`, {
        observe: 'response',
        params,
      })
      .subscribe({
        next: (res) => setPaginatedResponse(res, this.paginatedResult),
      });
  }

  getMessageThread(username: string) {
    return this.http.get<Message[]>(
      `${this.baseUrl}messages/thread/${username}`
    );
  }

  async sendMessage(username: string, content: string) {
    return this.hubsConnection?.invoke('SendMessage', {
      RecipentUserName: username,
      content,
    });
  }

  deleteMEssage(id: number) {
    return this.http.delete(`${this.baseUrl}messages/${id}`);
  }
}
