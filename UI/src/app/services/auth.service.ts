import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';
import { TokenApiModel } from '../models/token-api.model';
@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private baseUrl: string = 'https://localhost:7022/api/User/';
  private userPayload: any;
  constructor(private http: HttpClient, private router: Router) {
    this.userPayload = this.decodeToken();
  }

  signUp(user: any) {
    return this.http.post<any>(`${this.baseUrl}register`, user);
  }

  login(login: any) {
    return this.http.post<any>(`${this.baseUrl}authenticate`, login);
  }

  signOut() {
    localStorage.clear();
    this.router.navigate(['login']);
  }

  storeToken(tokenValue: string) {
    localStorage.setItem('token', tokenValue);
  }
  storeRefreshToken(tokenValue: string) {
    localStorage.setItem('refreshToken', tokenValue);
  }

  getToken() {
    return localStorage.getItem('token');
  }

  getRefreshToken() {
    return localStorage.getItem('refreshToken');
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem('token');
  }

  decodeToken() {
    const jwtHelper = new JwtHelperService();
    const token = this.getToken()!;
    console.log(jwtHelper.decodeToken(token));
    return jwtHelper.decodeToken(token);
  }

  getFullNameFromToken() {
    if (this.userPayload) return this.userPayload.name;
  }

  getRoleFromTOken() {
    if (this.userPayload) return this.userPayload.role;
  }
  renewToken(tokenApi: TokenApiModel) {
    return this.http.post<any>(`${this.baseUrl}refresh`, tokenApi);
  }

  getCurrentUser() {
    const jwtHelper = new JwtHelperService();
    const token = this.getToken();
    if (token) {
      const decodedToken = jwtHelper.decodeToken(token);
      return decodedToken;
    } else {
      return null;
    }
  }

}
