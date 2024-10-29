import { Component, inject, OnInit } from '@angular/core';
import { PageChangedEvent, PaginationModule } from 'ngx-bootstrap/pagination';
import { ButtonsModule } from 'ngx-bootstrap/buttons';

import { MembersService } from '../../_services/members.service';
import { MemberCardComponent } from '../member-card/member-card.component';
import { UserParam } from '../../_models/UserParam';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-member-list',
  standalone: true,
  imports: [MemberCardComponent, PaginationModule, FormsModule, ButtonsModule],
  templateUrl: './member-list.component.html',
  styleUrl: './member-list.component.css',
})
export class MemberListComponent implements OnInit {
  membersService = inject(MembersService);

  genderList = [
    { value: 'male', display: 'Males' },
    { value: 'female', display: 'Females' },
  ];

  ngOnInit(): void {
    if (!this.membersService.paginatedResult()) {
      this.loadMembers();
    }
  }

  loadMembers() {
    this.membersService.getMembers();
  }

  resetFilters() {
    this.membersService.resetUserParams();
    this.loadMembers();
  }

  pageChanged(event: PageChangedEvent) {
    if (this.membersService.userPrams().pageNumber !== event.page) {
      this.membersService.userPrams().pageNumber = event.page;
      this.loadMembers();
    }
  }
}
