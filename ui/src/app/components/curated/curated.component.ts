import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CuratedJokes } from '@app/models/curatedJokes.model';
import { CuratedJokeDataService } from '@app/services/joke-curated-data.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-curated-joke',
  templateUrl: 'curated.component.html',
  styleUrls: ['curated.component.css'],
})
export class CuratedComponent {
  curatedJokes$: Observable<CuratedJokes>;
  form: FormGroup;

  constructor(formbuilder: FormBuilder,
    private readonly jokeCuratedDataService: CuratedJokeDataService
  ) {
    this.form = formbuilder.group({
      searchTerm: ['', Validators.required]
    });
  }

  get searchTerm() { return this.form.get('searchTerm').value; }

  requestCuratedJokes() {
    this.curatedJokes$ = this.jokeCuratedDataService.getCuratedJokes(this.form.value.searchTerm)
    // this.form.reset();
  }
}
