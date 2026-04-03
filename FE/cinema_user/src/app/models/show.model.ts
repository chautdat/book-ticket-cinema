import { Time } from "@angular/common";

export class Show {
  movieId: number;
  movieName: string;
  moviePhoto: string;
  movieLanguage: string;
  movieGenre: string;
  movieTimeLast: Time;
  showTimes: [
    {
      id: number;
      price: number;
      dateRelease: Date;
      startTime: Date;
    }
  ];
}

export class ShowDetail {
  id: number;
  price: number;
  dateRelease: Date;
  startTime: Time;
  movie: {
    name: string;
    timeLast: Time;
    photo: string;
    language: string;
    genre: string;
    description: string;
  };
  room: {
    id: number;
    name: string;
    row: number;
    col: number;
  };
  seatStatues: [
    {
      id: number;
      status: boolean;
      seat: {
        id: number;
        roomId: number;
        name: string;
        row: number;
        col: number;
        status: boolean;
      };
    }
  ];
}
