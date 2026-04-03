import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { lastValueFrom } from 'rxjs';
import { BaseUrlService } from './baseUrl.service';

@Injectable()
export class FollowService {
  constructor(
    private baseUrlService: BaseUrlService,
    private httpClient: HttpClient
  ) {}

  async create(payload: any): Promise<any> {
    return await lastValueFrom(
      this.httpClient.post(this.baseUrlService.getBaseUrl() + 'follow/create', payload)
    );
  }

  async findById(id: number): Promise<any> {
    return await lastValueFrom(
      this.httpClient.get(this.baseUrlService.getBaseUrl() + 'follow/findById/' + id)
    );
  }
}
