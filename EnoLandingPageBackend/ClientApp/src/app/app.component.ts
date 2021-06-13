import { Component } from '@angular/core';
import { DataService } from 'projects/backend-api/src/lib';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent {
  public data: any;
  constructor(private dataService: DataService) {
    this.dataService.apiDataCtfInfoGet().subscribe((data) => {
      this.data = data;
    });
  }
  ngOnInit() {}
}
