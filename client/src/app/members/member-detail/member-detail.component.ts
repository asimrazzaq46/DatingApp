import { Component, inject, OnInit, signal, ViewChild } from '@angular/core';
import { MembersService } from '../../_services/members.service';
import { Member } from '../../_models/Member.model';
import { ActivatedRoute } from '@angular/router';
import { TabDirective, TabsetComponent, TabsModule } from 'ngx-bootstrap/tabs';
import { GalleryModule, GalleryItem, ImageItem } from 'ng-gallery';
import { TimeagoModule } from 'ngx-timeago';
import { DatePipe } from '@angular/common';
import { Photo } from '../../_models/Photo.model';
import { MemberMessagesComponent } from '../member-messages/member-messages.component';
import { Message } from '../../_models/Message.model';
import { MessageService } from '../../_services/message.service';

@Component({
  selector: 'app-member-detail',
  standalone: true,
  imports: [
    TabsModule,
    GalleryModule,
    TimeagoModule,
    DatePipe,
    MemberMessagesComponent,
  ],
  templateUrl: './member-detail.component.html',
  styleUrl: './member-detail.component.css',
})
export class MemberDetailComponent implements OnInit {
  @ViewChild('membersTab', { static: true }) membersTab?: TabsetComponent;

  private memberService = inject(MembersService);
  private messageService = inject(MessageService);
  private route = inject(ActivatedRoute);

  member: Member = {} as Member;
  images: GalleryItem[] = [];
  activeTab = signal<TabDirective | null>(null);
  messages = signal<Message[]>([]);

  ngOnInit(): void {
    this.route.data.subscribe({
      next: (data) => {
        this.member = data['member'];
        this.member &&
          this.member.photos.map((image: Photo) => {
            this.images.push(
              new ImageItem({ src: image.url, thumb: image.url })
            );
          });
      },
    });

    this.route.queryParams.subscribe({
      next: (params) => {
        params['tab'] && this.selectTab(params['tab']);
      },
    });
  }

  selectTab(heading: string) {
    if (this.membersTab) {
      const messageTab = this.membersTab?.tabs.find(
        (x) => x.heading === heading
      );
      if (messageTab) {
        messageTab.active = true;
      }
    }
  }

  onTabActivated(data: TabDirective) {
    this.activeTab.set(data);
    if (
      this.activeTab()?.heading === 'Messages' &&
      this.messages.length === 0 &&
      this.member
    ) {
      this.messageService.getMessageThread(this.member.userName).subscribe({
        next: (message) => this.messages.set(message),
      });
    }
  }

  onUpdateMessages(message: Message) {
    this.messages.update((prevMessages) => [...prevMessages, message]);
  }

  // loadUser() {
  //   const username = this.route.snapshot.paramMap.get('username');
  //   if (!username) return;
  //   this.memberService.getMember(username).subscribe({
  //     next: (res) => {
  //       if (res) {
  //         this.member = res;
  //         res.photos.map((image: Photo) => {
  //           this.images.push(
  //             new ImageItem({ src: image.url, thumb: image.url })
  //           );
  //         });
  //       }
  //     },
  //   });
  // }
}
