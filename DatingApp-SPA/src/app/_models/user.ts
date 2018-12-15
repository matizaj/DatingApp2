import {Photo} from './Photo';

export interface User {
    id: number;
    name: string;
    age: number;
    gender: string;
    created: Date;
    knownAs: string;
    lastActive: Date;
    photoUrl: string;
    city: string;
    country: string;
    interests?: string;
    introduction?: string;
    lookingFor?: string;
    photos?: Photo[];


}
