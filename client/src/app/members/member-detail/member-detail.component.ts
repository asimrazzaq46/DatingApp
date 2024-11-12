import {
  Component,
  computed,
  inject,
  OnDestroy,
  OnInit,
  signal,
  ViewChild,
} from '@angular/core';
import { Member } from '../../_models/Member.model';
import { ActivatedRoute, Router } from '@angular/router';
import { TabDirective, TabsetComponent, TabsModule } from 'ngx-bootstrap/tabs';
import { GalleryModule, GalleryItem, ImageItem } from 'ng-gallery';
import { TimeagoModule } from 'ngx-timeago';
import { DatePipe } from '@angular/common';
import { Photo } from '../../_models/Photo.model';
import { MemberMessagesComponent } from '../member-messages/member-messages.component';
import { Message } from '../../_models/Message.model';
import { MessageService } from '../../_services/message.service';
import { PresenceService } from '../../_services/presence.service';
import { AccountService } from '../../_services/account.service';
import { merge } from 'rxjs';
import { HubConnection, HubConnectionState } from '@microsoft/signalr';
import { LikesService } from '../../_services/likes.service';

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
export class MemberDetailComponent implements OnInit, OnDestroy {
  @ViewChild('membersTab', { static: true }) membersTab?: TabsetComponent;

  private auth = inject(AccountService);
  private messageService = inject(MessageService);
  presenceService = inject(PresenceService);
  private likeService = inject(LikesService);

  private route = inject(ActivatedRoute);
  private router = inject(Router);

  member: Member = {} as Member;
  images: GalleryItem[] = [];
  activeTab = signal<TabDirective | null>(null);
  hasLiked = computed(() =>
    this.likeService.likeIds().includes(this.member.id)
  );

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

    this.route.paramMap.subscribe({
      next: () => this.onRouteParamsChange(),
    });

    this.route.queryParams.subscribe({
      next: (params) => {
        params['tab'] && this.selectTab(params['tab']);
      },
    });
  }

  onlike() {
    this.likeService.onToggleLike(this.member, this.hasLiked());
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

  onRouteParamsChange() {
    const user = this.auth.currentUser();
    if (!user) return;
    if (
      this.messageService.hubsConnection?.state ===
        HubConnectionState.Connected &&
      this.activeTab()?.heading === 'Messages'
    ) {
      this.messageService.hubsConnection
        .stop()
        .then(() =>
          this.messageService.createHubConnection(user, this.member.userName)
        );
    }
  }

  onTabActivated(data: TabDirective) {
    this.activeTab.set(data);
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { tab: this.activeTab()?.heading },
      queryParamsHandling: 'merge',
    });
    if (this.activeTab()?.heading === 'Messages' && this.member) {
      const user = this.auth.currentUser();
      if (!user) return;
      this.messageService.createHubConnection(user, this.member.userName);
    } else {
      this.messageService.stopHubConnection();
    }
  }

  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
  }
}
