import {
  HttpClient,
  HttpHeaders,
  HttpParams,
  HttpResponse,
} from '@angular/common/http';
import { inject, Injectable, model, signal } from '@angular/core';
import { Member } from '../_models/Member.model';
import { UserParam } from '../_models/UserParam';
import { environment } from '../../environments/environment';
import { map, of, tap } from 'rxjs';
import { Photo } from '../_models/Photo.model';
import { PaginatedResult } from '../_models/Pagination.model';
import { User } from '../_models/User.model';
import { AccountService } from './account.service';
import {
  setPaginatedResponse,
  setPaginationHeaders,
} from './paginationHelpers';

@Injectable({
  providedIn: 'root',
})
export class MembersService {
  private http = inject(HttpClient);
  private auth = inject(AccountService);
  baseurl = environment.apiUrl;
  user = this.auth.currentUser();
  paginatedResult = signal<PaginatedResult<Member[]> | null>(null);
  memberCache = new Map(); // MAP is used to set and get values
  userPrams = signal<UserParam>(new UserParam(this.user));

  resetUserParams() {
    this.userPrams.set(new UserParam(this.user));
  }

  getMembers() {
    const response = this.memberCache.get(
      Object.values(this.userPrams()).join(' - ')
    );
    //caching filters and results
    if (response) return setPaginatedResponse(response, this.paginatedResult); // getting the cache

    let params = setPaginationHeaders(
      this.userPrams().pageNumber,
      this.userPrams().pageSize
    );

    params = params.append('minAge', this.userPrams().minAge);
    params = params.append('maxAge', this.userPrams().maxAge);
    params = params.append('gender', this.userPrams().gender);
    params = params.append('orderBy', this.userPrams().orderBy);

    return this.http
      .get<Member[]>(this.baseurl + 'users', { observe: 'response', params })
      .subscribe({
        next: (res) => {
          setPaginatedResponse(res, this.paginatedResult);
          this.memberCache.set(
            Object.values(this.userPrams()).join(' - '),
            res
          ); // setting the cache
        },
      });
  }

  getMember(username: string) {
    const member: Member = [...this.memberCache.values()]
      .reduce((arr, elem) => arr.concat(elem.body), [])
      .find((m: Member) => m.userName === username);

    if (member) return of(member);

    return this.http.get<Member>(this.baseurl + 'users/' + username);
  }

  updateMember(member: Member) {
    return this.http
      .put(this.baseurl + 'users', member)
      .pipe
      // tap(() =>
      //   this.members.update((prevMembers) =>
      //     prevMembers.map((m) =>
      //       m.userName === member.userName ? member : m
      //     )
      //   )
      // )
      ();
  }

  setMainPhoto(photo: Photo) {
    return this.http
      .put(this.baseurl + 'users/set-main-photo/' + photo.id, {})
      .pipe
      // tap(() => {
      //   //update the members and map on every member to update that one photo we have clicked
      //   this.members.update((prevMembers) =>
      //     prevMembers.map((member) => {
      //       //if the photo we clicked is inside the previou membersPhoto...than we will change the url
      //       if (member.photos.includes(photo)) {
      //         member.photoUrl = photo.url;
      //       }
      //       return member;
      //     })
      //   );
      // })
      ();
  }

  DeletePhoto(photo: Photo) {
    return this.http
      .delete(this.baseurl + 'users/delete-photo/' + photo.id)
      .pipe
      // tap(() => {
      //   this.members.update((prevMembers) =>
      //     prevMembers.map((m) => {
      //       if (m.photos.includes(photo)) {
      //         m.photos = m.photos.filter((p) => p.id !== photo.id);
      //       }
      //       return m;
      //     })
      //   );
      // })
      ();
  }
}
