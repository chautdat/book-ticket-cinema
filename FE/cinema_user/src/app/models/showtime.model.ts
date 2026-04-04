export class ShowTime{
    id: number;
    cinemaName: string;
    subId: number;
    subName: string;
    showDate: string;
    roomId?: number;
    totalSeats?: number;
    bookedSeats?: number;
    reservedSeats?: number;
    availableSeats?: number;
    isSoldOut?: boolean;
}
export class ShowTimeDetails{
    id: number;
    title: string;
    description: string;
    duration: string;
    genre: string;
    releaseDate: string;
    age: number;
    trailer: string;
    actor: string;
    director: string;
    publisher: string;
    photo: string;
    cinemaName: string;
    showDate: string;
    showTime: string;
    subId: number;
    subName: string;

}
