import { Component, Input, OnInit } from '@angular/core';
import { TeamMessage } from 'projects/backend-api/src/lib';

@Component({
  selector: 'app-team',
  templateUrl: './team.component.html',
  styleUrls: ['./team.component.scss'],
})
export class TeamComponent implements OnInit {
  @Input()
  public team!: TeamMessage;
  constructor() {}

  ngOnInit(): void {}
}
