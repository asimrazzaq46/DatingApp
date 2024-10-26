import {
  Component,
  HostListener,
  inject,
  OnInit,
  viewChild,
} from '@angular/core';
import { Member } from '../../_models/Member.model';
import { AccountService } from '../../_services/account.service';
import { MembersService } from '../../_services/members.service';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { FormsModule, NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { PhotoEditorComponent } from '../photo-editor/photo-editor.component';

@Component({
  selector: 'app-member-edit',
  standalone: true,
  imports: [TabsModule, FormsModule, PhotoEditorComponent],
  templateUrl: './member-edit.component.html',
  styleUrl: './member-edit.component.css',
})
export class MemberEditComponent implements OnInit {
  editForm = viewChild<NgForm>('editForm');
  @HostListener('window:beforeunload', ['$event']) notify($event: any) {
    if (this.editForm()?.dirty) {
      $event.returnValue = true;
    }
  }

  member?: Member;

  private auth = inject(AccountService);
  private memberService = inject(MembersService);
  private toaster = inject(ToastrService);

  ngOnInit(): void {
    this.loadUser();
  }

  loadUser() {
    const user = this.auth.currentUser();
    if (!user) return;

    this.memberService.getMember(user.username).subscribe({
      next: (user) => (this.member = user),
    });
  }

  updateMember() {
    this.memberService.updateMember(this.editForm()?.value).subscribe({
      next: (_) => {
        this.toaster.success('Profile Updated Successfully');
        this.editForm()?.reset(this.member);
      },
    });
  }

  onMemberChange(event: Member) {
    this.member = event;
  }
}
