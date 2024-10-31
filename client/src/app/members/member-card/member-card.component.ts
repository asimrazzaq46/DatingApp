import { Component, computed, inject, input, OnInit } from '@angular/core';
import { Member } from '../../_models/Member.model';
import { RouterLink } from '@angular/router';
import { LikesService } from '../../_services/likes.service';

@Component({
  selector: 'app-member-card',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './member-card.component.html',
  styleUrl: './member-card.component.css',
})
export class MemberCardComponent {
  private likeService = inject(LikesService);

  members = input.required<Member>();

  hasLiked = computed(() =>
    this.likeService.likeIds().includes(this.members().id)
  );

  toggleLike() {
    this.likeService.toggleLike(this.members().id).subscribe({
      next: () => {
        if (this.hasLiked()) {
          this.likeService.likeIds.update((prevId) =>
            prevId.filter((x) => x !== this.members().id)
          );
        } else {
          this.likeService.likeIds.update((prevIds) => [
            ...prevIds,
            this.members().id,
          ]);
        }
      },
    });
  }
} 
