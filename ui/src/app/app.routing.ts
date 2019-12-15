import { Routes } from '@angular/router';
import { RandomComponent } from './components/random/random.component';
import { CuratedComponent } from './components/curated/curated.component';

export const appRoutes: Routes = [
  { path: '', redirectTo: 'random', pathMatch: 'full' },
  { path: 'random', component: RandomComponent },
  { path: 'curated', component: CuratedComponent }
];
