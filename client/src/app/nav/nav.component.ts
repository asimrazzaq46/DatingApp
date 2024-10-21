import { CommonModule, TitleCasePipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { BsDropdownConfig, BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { Toast, ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-nav',
  standalone: true,
  imports: [
    FormsModule,
    BsDropdownModule,
    RouterLink,
    RouterLinkActive,
    TitleCasePipe,
  ],
  templateUrl: './nav.component.html',
  styleUrl: './nav.component.css',
})
export class NavComponent {
  auth = inject(AccountService);
  model: any = {};
  user: string = '';

  private router = inject(Router);
  private toaster = inject(ToastrService);

  login() {
    this.auth.login(this.model).subscribe({
      next: (_) => this.router.navigateByUrl('/members'),
      error: (err) => this.toaster.error(err.error),
    });
  }

  logout() {
    this.auth.logout();
    this.router.navigateByUrl('/');
  }
}
