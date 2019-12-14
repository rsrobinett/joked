import { Component, OnInit } from '@angular/core';
import { CuratedJokeDataService } from '@app/services/joke-curated-data.service';
import { SignalRService } from '@app/services/signalR.service';
import { Observable } from 'rxjs';
import { CuratedJokes } from '@app/models/curatedJokes.model';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  signalrConnectionEstablished$: Observable<boolean>;
  curatedJokes$: Observable<CuratedJokes>;
  jokeValue$: Observable<string>;
  constructor(
    private readonly signalRService: SignalRService,
    private readonly jokeCuratedDataService: CuratedJokeDataService
  ) {}

  ngOnInit() {
    this.signalrConnectionEstablished$ = this.signalRService.connectionEstablished$;
    this.jokeValue$ = this.signalRService.randomJoke$;
  }
  
 requestCuratedJokes(searchTerm:string) {
    this.curatedJokes$ = this.jokeCuratedDataService.getCuratedJokes(searchTerm)
   }
}
