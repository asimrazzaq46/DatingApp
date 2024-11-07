import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import {
  HubConnection,
  HubConnectionBuilder,
  HubConnectionState,
} from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { User } from '../_models/User.model';
import { take } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class PresenceService {
  hubsUrl = environment.hubsUrl;
  private hubsConnection?: HubConnection;

  private toastr = inject(ToastrService);
  private router = inject(Router);

  onlineUsers = signal<string[]>([]);

  createHubsConnection(user: User) {
    this.hubsConnection = new HubConnectionBuilder()
      .withUrl(`${this.hubsUrl}presence`, {
        accessTokenFactory: () => user.token,
      })
      .withAutomaticReconnect()
      .build();

    this.hubsConnection.start().catch((error) => console.log(error));

    this.hubsConnection.on('UserIsOnline', (username) => {
      this.onlineUsers.update((prevUsers) => [...prevUsers, username]);
    });

    this.hubsConnection.on('UserIsOffline', (username) => {
      this.onlineUsers.update((prevUsers) =>
        prevUsers.filter((u) => u !== username)
      );
    });

    this.hubsConnection.on('GetOnlineUsers', (users) => {
      this.onlineUsers.set(users);
    });

    this.hubsConnection.on('NewMessageRecieved', ({ username, knownAs }) => {
      this.toastr
        .info(knownAs + ' has sent you a message. Click me to see it')
        .onTap.pipe(take(1))
        .subscribe(() =>
          this.router.navigateByUrl(`/members/${username}?tab=Messages`)
        );
    });
  }
  stopHubConnection() {
    if (this.hubsConnection?.state === HubConnectionState.Connected) {
      this.hubsConnection.stop().catch((err) => console.log(err));
    }
  }
}
