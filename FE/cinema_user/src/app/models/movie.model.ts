import { ShowTime } from "./showtime.model";
import { Sub } from "./sub.model";

export class Movie{
    showtimes: ShowTime[]
    id: number;
    title: string;
    description: string;
    duration: string;
    genre: string;
    trailer: string;
    actor: string;
    publisher: string;
    releaseDate: string;
    photo: string;
    status: boolean;
    age: number;
    director: string;
    listSubId: number[];
    listSubName: string[];
    subs: ShowTime[];
}