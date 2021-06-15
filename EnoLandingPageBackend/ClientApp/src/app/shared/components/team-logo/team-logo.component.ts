import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-team-logo',
  templateUrl: './team-logo.component.html',
  styleUrls: ['./team-logo.component.scss'],
})
export class TeamLogoComponent implements OnInit {
  @Input()
  logoUrl!: string;
  constructor() {}

  ngOnInit(): void {}
}
