import { Component, inject, OnInit } from '@angular/core';
import { RegisterComponent } from '../register/register.component';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RegisterComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
})
export class HomeComponent implements OnInit {
  registerMode: boolean = false;

  http = inject(HttpClient);
  url = 'https://localhost:5001/api/users';
  users: any = [];

  ngOnInit(): void {
    this.getUsers();
  }
  
  registerToggle() {
    this.registerMode = !this.registerMode;
  }

  getUsers() {
    this.http.get(this.url).subscribe({
      next: (res) => (this.users = res),
    });
  }

  onCancel($event: boolean) {
    this.registerMode = $event;
  }
}
