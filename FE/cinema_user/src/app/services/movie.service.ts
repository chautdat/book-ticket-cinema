import { Injectable } from "@angular/core";
import { BaseUrlService } from "./baseUrl.service";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom } from "rxjs";

@Injectable()
export class MovieService{
    constructor(
        private baseUrlService: BaseUrlService,
        private httpClient: HttpClient
    ){}
    async findAll(date: string, id: number) : Promise<any>{
        return await lastValueFrom(this.httpClient.get(this.baseUrlService.getBaseUrl()
        + 'movie/findAll?date='+ date.split(' ')[0] + '&cinemaId=' + id));
    }

    async findAllByStatus() : Promise<any>{
        return await lastValueFrom(this.httpClient.get(this.baseUrlService.getBaseUrl()
        + 'movie/findAllByStatus'));
    }

    async findMovieById(id: number) : Promise<any>{
        return await lastValueFrom(this.httpClient.get(this.baseUrlService.getBaseUrl()
        + 'movie/findMovieById/' + id));
    }

    async findMovie(date: string, cinemaId: number, movieId: number) : Promise<any>{
        return await lastValueFrom(this.httpClient.get(this.baseUrlService.getBaseUrl()
        + 'movie/findMovie?date='+ date + '&cinemaId=' + cinemaId + '&movieId=' + movieId));
    }
}