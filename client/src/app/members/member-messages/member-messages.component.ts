import {
  Component,
  inject,
  input,
  OnInit,
  output,
  signal,
  ViewChild,
} from '@angular/core';
import { MessageService } from '../../_services/message.service';
import { AccountService } from '../../_services/account.service';
import { Message } from '../../_models/Message.model';
import { TimeagoModule } from 'ngx-timeago';
import { FormsModule, NgForm } from '@angular/forms';

@Component({
  selector: 'app-member-messages',
  standalone: true,
  imports: [TimeagoModule, FormsModule],
  templateUrl: './member-messages.component.html',
  styleUrl: './member-messages.component.css',
})
export class MemberMessagesComponent {
  private messageService = inject(MessageService);

  username = input.required<string>();
  messages = input.required<Message[]>();
  messageContent = signal<string>('');
  updateMessages = output<Message>();

  @ViewChild('messageForm') messageForm?: NgForm;

  sendMessage() {
    this.messageService
      .sendMessage(this.username(), this.messageContent())
      .subscribe({
        next: (message) => {
          this.updateMessages.emit(message);
          this.messageForm?.reset();
        },
      });
  }
}
