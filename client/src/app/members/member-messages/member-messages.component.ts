import {
  AfterContentChecked,
  AfterViewChecked,
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  ElementRef,
  inject,
  input,
  IterableDiffers,
  OnInit,
  signal,
  ViewChild,
} from '@angular/core';
import { MessageService } from '../../_services/message.service';

import { TimeagoModule } from 'ngx-timeago';
import { FormsModule, NgForm } from '@angular/forms';

@Component({
  selector: 'app-member-messages',
  standalone: true,
  imports: [TimeagoModule, FormsModule],
  templateUrl: './member-messages.component.html',
  styleUrl: './member-messages.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MemberMessagesComponent implements AfterContentChecked, OnInit {
  ngOnInit(): void {
    this.scrollToBottom();
  }
  messageService = inject(MessageService);
  private cdref = inject(ChangeDetectorRef);

  username = input.required<string>();
  messageContent = signal<string>('');

  @ViewChild('messageForm') messageForm?: NgForm;
  @ViewChild('scrollMe') scrollMe?: ElementRef;

  sendMessage() {
    this.messageService
      .sendMessage(this.username(), this.messageContent())
      .then(() => this.messageForm?.reset());
  }

  ngAfterContentChecked(): void {
    this.cdref.detectChanges();
  }

  private scrollToBottom() {
    if (this.scrollMe) {
      this.scrollMe.nativeElement.scrollTop =
        this.scrollMe.nativeElement.scrollHeight;
    }
  }
}
