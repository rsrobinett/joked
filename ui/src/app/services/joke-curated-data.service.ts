import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@environments/environment';
import { throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { CuratedJokes } from '../models/curatedJokes.model';

@Injectable({ providedIn: 'root' })
export class CuratedJokeDataService {
  private actionUrl: string;

  constructor(private http: HttpClient) {
    this.actionUrl =
      environment.baseUrls.server + environment.baseUrls.apiUrl + 'jokes?curate=true&limit=30';
  }

  getCuratedJokes(searchTerm: string) {
      return this.http
      .get<CuratedJokes>(this.actionUrl + "&term=" + searchTerm)
      .pipe(catchError(this.handleError));
  }

  private handleError(error: Response) {
    return throwError(error || 'Server error');
  }
}
