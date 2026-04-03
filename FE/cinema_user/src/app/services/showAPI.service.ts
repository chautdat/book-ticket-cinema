import { Injectable } from "@angular/core";
import { BaseUrlService } from "./baseUrl.service";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
@Injectable()
export class ShowAPIService {
  constructor(
    private baseUrlService: BaseUrlService,
    private httpClient: HttpClient
  ) {}
  async getListByDateRelease(date: string) {
    return await lastValueFrom(
      this.httpClient.get(
        this.baseUrlService.getBaseUrl() + "shows/date/" + date
      )
    );
  }
  async getDetail(id: number) {
    return await lastValueFrom(
      this.httpClient.get(this.baseUrlService.getBaseUrl() + "shows/" + id)
    );
  }
}
