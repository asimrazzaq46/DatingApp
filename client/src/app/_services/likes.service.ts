import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member } from '../_models/Member.model';
import { PaginatedResult } from '../_models/Pagination.model';
import {
  setPaginatedResponse,
  setPaginationHeaders,
} from './paginationHelpers';

@Injectable({
  providedIn: 'root',
})
export class LikesService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;
  likeIds = signal<number[]>([]);
  paginatedResult = signal<PaginatedResult<Member[]> | null>(null);

  toggleLike(targetId: number) {
    return this.http.post(this.baseUrl + 'likes/' + targetId, {});
  }

  getLikes(predicate: string, pageNumber: number, pageSize: number) {
    let params = setPaginationHeaders(pageNumber, pageSize);
    params = params.append('predicate', predicate);
    return this.http
      .get<Member[]>(`${this.baseUrl}likes`, {
        observe: 'response',
        params,
      })
      .subscribe({
        next: (res) => setPaginatedResponse(res, this.paginatedResult),
      });
  }

  getLikesId() {
    return this.http.get<number[]>(`${this.baseUrl}likes/list`).subscribe({
      next: (id) => this.likeIds.set(id),
    });
  }
}
