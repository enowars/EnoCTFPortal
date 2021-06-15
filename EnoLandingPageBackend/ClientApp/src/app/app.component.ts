import { Component } from '@angular/core';
import { Store } from '@ngxs/store';
import { DataService } from 'projects/backend-api/src/lib';
import { AppState, InitTheme } from './shared/states/App.state';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent {
  public data: any;
  constructor(private store: Store) {
    this.store.select(AppState).subscribe((app) => {
      console.log(app);
    });
  }
  ngOnInit() {
    this.store.dispatch(new InitTheme());
  }
}
