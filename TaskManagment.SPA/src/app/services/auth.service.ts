import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment';
import { LoginRequest } from '../models/login-request.dto';
import { LoginResponse } from '../models/login-response.dto';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private key = 'tm_token';
  private roleKey = 'tm_role';

  constructor(private http: HttpClient, private router: Router) { }

  login(req: LoginRequest) {
    return this.http.post<LoginResponse>(`${environment.api}/auth/login`, req);
  }

  saveAuth(token: string, role: string) {
    localStorage.setItem(this.key, token);
    localStorage.setItem(this.roleKey, role);
  }

  token() { return localStorage.getItem(this.key); }
  role() { return localStorage.getItem(this.roleKey); }
  isLoggedIn() { return !!this.token(); }

  logout() {
    localStorage.removeItem(this.key);
    localStorage.removeItem(this.roleKey);
    this.router.navigate(['/login']);
  }
}
