import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import {
  ScoreboardService,
  ServiceStatus,
} from 'projects/backend-api/src/lib/model/models';

export interface InfoDialogData {
  row: any;
  service: ScoreboardService;
}

@Component({
  selector: 'app-dialog-info',
  templateUrl: './dialog-info.component.html',
  styleUrls: ['./dialog-info.component.scss'],
})
export class DialogInfoComponent implements OnInit {
  public statusEnum: typeof ServiceStatus = ServiceStatus;
  constructor(@Inject(MAT_DIALOG_DATA) public data: InfoDialogData) {}

  ngOnInit(): void {}
}
