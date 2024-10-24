import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { Member } from '../_models/Member.model';
import { environment } from '../../environments/environment';
import { of, tap } from 'rxjs';

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
}
