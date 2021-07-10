import { Component } from '@angular/core';
import { Store } from '@ngxs/store';
import { DataService } from 'projects/backend-api/src/lib';
import { AppState, AppStateModel, InitTheme } from './shared/states/App.state';
import { filter } from 'rxjs/operators';
import { Title } from '@angular/platform-browser';
import { Fireworks } from 'fireworks-js';

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
    setTimeout(() => {
      console.log(this.store.selectSnapshot(AppState.ctfIsOver));
      if (this.store.selectSnapshot(AppState.ctfIsOver)) {
        let host = document.getElementById('firework')!;
        const fireworks = new Fireworks(host, {
          boundaries: {
            top: 50,
            bottom: host.clientHeight,
            left: 50,
            right: host.clientWidth,
          },
          mouse: { click: true, move: false, max: 3 },
        });
        fireworks.start();
      }
    }, 2000);
  }
}
