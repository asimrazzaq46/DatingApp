import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent implements OnInit {
  http = inject(HttpClient);
  url = 'https://localhost:5001/api/users';
  title = 'Dating App';

  ngOnInit(): void {
    this.http.get(this.url).subscribe({
      next: (res) => console.log(res),
    });
  }
}
