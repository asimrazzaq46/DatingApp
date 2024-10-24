import { Photo } from './Photo.model';

export interface Member {
  id: number;
  userName: string;
  age: number;
  photoUrl: string;
  knownAs: string;
  created: Date;
  lastActive: Date;
  gender: string;
  introduction: string;
  intrests: string;
  lookingFor: string;
  city: string;
  country: string;
  photos: Photo[];
}
