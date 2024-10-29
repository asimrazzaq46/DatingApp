import { Component, inject, OnInit, ViewEncapsulation } from '@angular/core';
import { MembersService } from '../../_services/members.service';
import { Member } from '../../_models/Member.model';
import { ActivatedRoute } from '@angular/router';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { GalleryModule, GalleryItem, ImageItem } from 'ng-gallery';
import { TimeagoModule } from 'ngx-timeago';
import { DatePipe } from '@angular/common';
import { Photo } from '../../_models/Photo.model';

@Component({
  selector: 'app-member-detail',
  standalone: true,
  imports: [TabsModule, GalleryModule, TimeagoModule, DatePipe],
  templateUrl: './member-detail.component.html',
  styleUrl: './member-detail.component.css',
})
export class MemberDetailComponent implements OnInit {
  private memberService = inject(MembersService);
  private route = inject(ActivatedRoute);

  member?: Member;
  images: GalleryItem[] = [];
  ngOnInit(): void {
    this.loadUser();
  }

  loadUser() {
    const username = this.route.snapshot.paramMap.get('username');
    if (!username) return;
    this.memberService.getMember(username).subscribe({
      next: (res) => {
        if (res) {
          this.member = res;
          res.photos.map((image: Photo) => {
            this.images.push(
              new ImageItem({ src: image.url, thumb: image.url })
            );
          });
        }
      },
    });
  }
}
