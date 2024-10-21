import { Component, inject, input, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent {
  model: any = {};

  private auth = inject(AccountService);
  private toster = inject(ToastrService);

  cancelRegister = output<boolean>();

  register() {
    this.auth.register(this.model).subscribe({
      next: (res) => {
        console.log(res);
        this.cancel();
      },
      error: (err) => this.toster.error(err.error),
    });
  }

  cancel() {
    this.cancelRegister.emit(false);
  }
}
