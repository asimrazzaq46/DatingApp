import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { Member } from '../_models/Member.model';
import { environment } from '../../environments/environment';
import { of, tap } from 'rxjs';
import { Photo } from '../_models/Photo.model';

@Injectable({
  providedIn: 'root',
})
export class MembersService {
  private http = inject(HttpClient);
  baseurl = environment.apiUrl;
  members = signal<Member[]>([]);

  getMembers() {
    return this.http.get<Member[]>(this.baseurl + 'users').subscribe({
      next: (res) => this.members.set(res),
    });
  }

  getMember(username: string) {
    const member = this.members().find((u) => u.userName === username);
    if (member != undefined) return of(member);

    return this.http.get<Member>(this.baseurl + 'users/' + username);
  }

  updateMember(member: Member) {
    return this.http
      .put(this.baseurl + 'users', member)
      .pipe(
        tap(() =>
          this.members.update((prevMembers) =>
            prevMembers.map((m) =>
              m.userName === member.userName ? member : m
            )
          )
        )
      );
  }

  setMainPhoto(photo: Photo) {
    return this.http
      .put(this.baseurl + 'users/set-main-photo/' + photo.id, {})
      .pipe(
        tap(() => {
          //update the members and map on every member to update that one photo we have clicked
          this.members.update((prevMembers) =>
            prevMembers.map((member) => {
              //if the photo we clicked is inside the previou membersPhoto...than we will change the url
              if (member.photos.includes(photo)) {
                member.photoUrl = photo.url;
              }
              return member;
            })
          );
        })
      );
  }

  DeletePhoto(photo: Photo) {
    return this.http
      .delete(this.baseurl + 'users/delete-photo/' + photo.id)
      .pipe(
        tap(() => {
          this.members.update((prevMembers) =>
            prevMembers.map((m) => {
              if (m.photos.includes(photo)) {
                m.photos = m.photos.filter((p) => p.id !== photo.id);
              }
              return m;
            })
          );
        })
      );
  }
}
