import { Component, inject, input, OnInit, output } from '@angular/core';
import { Member } from '../../_models/Member.model';
import {
  DecimalPipe,
  JsonPipe,
  NgClass,
  NgFor,
  NgIf,
  NgStyle,
} from '@angular/common';
import { FileUploader, FileUploadModule } from 'ng2-file-upload';
import { AccountService } from '../../_services/account.service';
import { environment } from '../../../environments/environment';
import { Photo } from '../../_models/Photo.model';
import { MembersService } from '../../_services/members.service';

@Component({
  selector: 'app-photo-editor',
  standalone: true,
  imports: [
    NgStyle,
    NgIf,
    NgFor,
    NgClass,
    FileUploadModule,
    DecimalPipe,
    JsonPipe,
  ],
  templateUrl: './photo-editor.component.html',
  styleUrl: './photo-editor.component.css',
})
export class PhotoEditorComponent implements OnInit {
  member = input.required<Member>();
  baseUrl = environment.apiUrl;
  uploader?: FileUploader;
  hasBaseDropZoneOver: boolean = false;
  response?: string;
  memberChange = output<Member>();

  private auth = inject(AccountService);
  private memberService = inject(MembersService);

  ngOnInit(): void {
    this.initializeUploader();

    this.member().photos.forEach((p) => console.log(p.isApproved));
  }

  fileOverBase($event: any) {
    this.hasBaseDropZoneOver = $event;
  }

  setMainPhoto(photo: Photo) {
    this.memberService.setMainPhoto(photo).subscribe({
      next: (_) => {
        const user = this.auth.currentUser();
        if (user) {
          user.photoUrl = photo.url;
          this.auth.setCurrentUser(user);
        }

        const updatedMember = { ...this.member() };
        updatedMember.photoUrl = photo.url;
        updatedMember.photos.forEach((p) => {
          if (p.isMain) p.isMain = false;
          if (p.id === photo.id) p.isMain = true;
        });
        this.memberChange.emit(updatedMember);
      },
    });
  }

  deletePhotobyId(photo: Photo) {
    this.memberService.DeletePhoto(photo).subscribe({
      next: (_) => {
        const updatedMember = { ...this.member() };
        updatedMember.photos = updatedMember.photos.filter(
          (p) => p.id !== photo.id
        );
        this.memberChange.emit(updatedMember);
      },
    });
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/add-photo',
      authToken: 'Bearer ' + this.auth.currentUser()?.token,
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: false,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024,
    });
    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    };
    this.uploader.onSuccessItem = (item, response, statis, headers) => {
      const photo = JSON.parse(response);

      const updatedMember = { ...this.member() };
      updatedMember.photos.push(photo);
      this.memberChange.emit(updatedMember);
      if (photo.isMain) {
        const user = this.auth.currentUser();
        if (user) {
          user.photoUrl = photo.photoUrl;
          this.auth.setCurrentUser(user);
        }
        updatedMember.photoUrl = photo.url;
        updatedMember.photos.forEach((p) => {
          if (p.isMain) p.isMain = false;
          if (p.id === photo.id) p.isMain = true;
        });
        this.memberChange.emit(updatedMember);
      }

      this.uploader!.queue = [];
    };
  }

  onClick() {
    console.log(`i am clicked`);
  }
}
