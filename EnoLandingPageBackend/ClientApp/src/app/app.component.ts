import { Component } from '@angular/core';
import { Store } from '@ngxs/store';
import { DataService } from 'projects/backend-api/src/lib';
import { InitTheme } from './shared/states/App.state';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent {
  public data: any;
  constructor(private store: Store, private dataService: DataService) {
    this.dataService.apiDataCtfInfoGet().subscribe((data) => {
      this.data = data;
    });
  }
  ngOnInit() {
    this.store.dispatch(new InitTheme());
  }
}
