// auth.service.ts
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BaseUrlService } from './baseUrl.service';
import { Observable, lastValueFrom } from 'rxjs';


@Injectable()
export class AuthService {


  private apiUrl = 'http://localhost:5113/api/google'; 


  constructor(
    private httpClient: HttpClient
  ) {}

  googleLogin(idToken: string): Observable<any> {
    return this.httpClient.post<any>(`${this.apiUrl}/google-login`, { idToken });
  }

  googleRegister(idToken: string): Observable<any> {
    return this.httpClient.post<any>(`${this.apiUrl}/google-register`, { idToken });
  }
}
