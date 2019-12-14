import { Component, EventEmitter, Input, Output, NgModule } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CuratedJokes } from '@app/models/curatedJokes.model';
import { MatTabsModule } from '@angular/material';

@Component({
  selector: 'app-curated',
  templateUrl: 'curated.component.html',
  styleUrls: ['curated.component.css'],
})
export class CuratedJokeComponent {
  @Input() curatedJokes: CuratedJokes;
  @Input() connectionEstablished = false;

  @Output() curatedJokesRequested = new EventEmitter();

  form: FormGroup;
  searchTerm: string
  
  constructor(formbuilder: FormBuilder) {
    this.form = formbuilder.group({
      id: null,
      searchTerm: ['', Validators.required]
    });
  }

  requestCuratedJokes(searchTerm : string) {
    this.curatedJokesRequested.emit(this.form.value.searchTerm);
    this.form.reset();
  }
}
