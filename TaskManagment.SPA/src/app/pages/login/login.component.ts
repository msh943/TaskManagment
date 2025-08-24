import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  email = 'admin@demo.com';
  password = 'Pass123$';
  error: string | null = null;

  constructor(private auth: AuthService, private router: Router) { }

  submit() {
    this.error = null;
    this.auth.login({ email: this.email, password: this.password }).subscribe({
      next: res => {
        this.auth.saveAuth(res.token, res.role);
        this.router.navigate(['/dashboard']);
      },
      error: _ => this.error = 'Invalid credentials'
    });
  }
}
