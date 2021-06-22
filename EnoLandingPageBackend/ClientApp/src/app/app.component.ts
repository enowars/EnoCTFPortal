import { Component } from '@angular/core';
import { Store } from '@ngxs/store';
import { DataService } from 'projects/backend-api/src/lib';
import { AppState, AppStateModel, InitTheme } from './shared/states/App.state';
import { filter } from 'rxjs/operators';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent {
  public data: any;
  constructor(private store: Store, private title: Title) {
    this.store.select(AppState).subscribe((app: AppStateModel) => {
      this.title.setTitle(app.ctfInfo?.title!);
      console.log(app);
    });
  }
  ngOnInit() {
    this.store.dispatch(new InitTheme());
  }
}
