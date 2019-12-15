import { Component, OnInit } from '@angular/core';
import { SignalRService } from '@app/services/signalR.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-random-joke',
  templateUrl: './random.component.html',
  styleUrls: ['./random.component.css']
})
export class RandomComponent implements OnInit {
  signalrConnectionEstablished$: Observable<boolean>;
  jokeValue$: Observable<string>;

  constructor(
    private readonly signalRService: SignalRService,
  ) { }

  ngOnInit() {
    this.signalrConnectionEstablished$ = this.signalRService.connectionEstablished$;
    this.jokeValue$ = this.signalRService.randomJoke$;
  }
}
