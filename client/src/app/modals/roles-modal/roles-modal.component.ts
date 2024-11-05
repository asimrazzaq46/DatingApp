import { Component, inject } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-roles-modal',
  standalone: true,
  imports: [],
  templateUrl: './roles-modal.component.html',
  styleUrl: './roles-modal.component.css',
})
export class RolesModalComponent {
  bsModalRef = inject(BsModalRef);

  username: string = '';
  title: string = '';
  availableRoles: string[] = [];
  selectedRoles: string[] = [];
  rolesUpdated = false;

  updateChecked(checkedValue: string) {
    if (this.selectedRoles.includes(checkedValue)) {
      this.selectedRoles = this.selectedRoles.filter(
        (value) => value !== checkedValue
      );
    } else {
      this.selectedRoles.push(checkedValue);
    }
  }

onSelectedRoles(){
  this.rolesUpdated = true;
  this.bsModalRef.hide()
}

}
