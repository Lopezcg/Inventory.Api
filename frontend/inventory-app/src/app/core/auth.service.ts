import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';

interface LoginRequest {
  username: string;
  password: string;
}

interface LoginResponse {
  accessToken: string;
  expiresAtUtc: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly apiUrl = 'http://localhost:5038';
  private readonly tokenStorageKey = 'inventory_token';

  constructor(private readonly http: HttpClient) {}

  login(username: string, password: string): Observable<void> {
    const payload: LoginRequest = { username, password };
    return this.http
      .post<LoginResponse>(`${this.apiUrl}/auth/login`, payload)
      .pipe(map((response) => localStorage.setItem(this.tokenStorageKey, response.accessToken)));
  }
}
