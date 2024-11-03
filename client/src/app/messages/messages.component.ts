import { Component, inject, OnInit } from '@angular/core';
import { MessageService } from '../_services/message.service';
import { FormsModule } from '@angular/forms';
import { PageChangedEvent, PaginationModule } from 'ngx-bootstrap/pagination';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { TimeagoModule } from 'ngx-timeago';
import { Message } from '../_models/Message.model';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-messages',
  standalone: true,
  imports: [
    FormsModule,
    PaginationModule,
    ButtonsModule,
    TimeagoModule,
    RouterLink,
  ],
  templateUrl: './messages.component.html',
  styleUrl: './messages.component.css',
})
export class MessagesComponent implements OnInit {
  messageService = inject(MessageService);
  container: 'Inbox' | 'Outbox' | 'Unread' = 'Inbox';
  pageNumber = 1;
  pageSize = 5;

  ngOnInit(): void {
    this.loadMessages();
  }

  loadMessages() {
    return this.messageService.getMessages(
      this.pageNumber,
      this.pageSize,
      this.container
    );
  }

  deleteMessage(id: number) {
    this.messageService.deleteMEssage(id).subscribe({
      next: (_) => {
        this.messageService.paginatedResult.update((prevMessages) => {
          if (prevMessages && prevMessages.items) {
            prevMessages.items.splice(
              prevMessages.items.findIndex((m) => m.id === id),
              1
            );
            return prevMessages;
          }
          return prevMessages;
        });
      },
    });
  }

  getRoutes(message: Message) {
    if (this.container === 'Outbox')
      return `/members/${message.recipentUsername}`;
    else return `/members/${message.senderUsername}`;
  }

  pageChanged(event: PageChangedEvent) {
    if (this.pageNumber !== event.page) {
      this.pageNumber = event.page;
      this.loadMessages();
    }
  }
}
