import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
  
  @Component({
    selector: 'app-random-joke',
    templateUrl: './joke.component.html',
    styleUrls: ['./joke.component.css'],

  })
  export class RandomJokeComponent implements OnChanges {
    @Input() jokeValue: string;
  
    view: any[] = [400, 400];
    data: string;
    requestCuratedJokes: string;

    colorScheme = {
      domain: ['#5AA454']
    };
  
    ngOnChanges(changes: SimpleChanges) {
      if (changes.jokeValue) {
        this.displayJoke(changes.jokeValue.currentValue);
      }
    }
  
    displayJoke(jokeValue: string) {
      this.data = jokeValue 
    }
  }
  