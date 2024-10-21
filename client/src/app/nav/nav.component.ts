import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { BsDropdownConfig, BsDropdownModule } from 'ngx-bootstrap/dropdown';

@Component({
  selector: 'app-nav',
  standalone: true,
  imports: [FormsModule, BsDropdownModule],
  templateUrl: './nav.component.html',
  styleUrl: './nav.component.css',
})
export class NavComponent {
   auth = inject(AccountService);
  model: any = {};
  user: string = '';

  login() {
    this.auth.login(this.model).subscribe({
      next: (res: any) => {
        this.user = res.username;
      },
      error: (err) => console.log(err.error),
    });
  }

  logout() {
    this.auth.logout();
  }
}
