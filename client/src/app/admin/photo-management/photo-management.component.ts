import { Component, inject, OnInit } from '@angular/core';
import { AdminService } from '../../_services/admin.service';
import { Photo } from '../../_models/Photo.model';

@Component({
  selector: 'app-photo-management',
  standalone: true,
  imports: [],
  templateUrl: './photo-management.component.html',
  styleUrl: './photo-management.component.css',
})
export class PhotoManagementComponent implements OnInit {
  photos: Photo[] = [];
  private adminService = inject(AdminService);

  ngOnInit(): void {
    this.PhotosForApprovalList();
  }

  PhotosForApprovalList() {
    this.adminService.getPhotosForApproval().subscribe({
      next: (photo) => (this.photos = photo),
    });
  }

  onApprove(Photid: number) {
    this.adminService.approvePhoto(Photid).subscribe({
      next: () =>
        this.photos.splice(this.photos.findIndex((p) => p.id === Photid),1),
    });
  }

  onReject(Photid: number) {
    this.adminService.rejectPhoto(Photid).subscribe({
      next: () =>
        this.photos.splice(this.photos.findIndex((p) => p.id === Photid),1),
    });
  }
}
