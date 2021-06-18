import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

export interface InfoDialogData {
  [str: string]: any
}

@Component({
  selector: 'app-dialog-info',
  templateUrl: './dialog-info.component.html',
  styleUrls: ['./dialog-info.component.scss']
})
export class DialogInfoComponent implements OnInit {

  constructor(@Inject(MAT_DIALOG_DATA) public info: InfoDialogData) {}

  ngOnInit(): void {
  }

}
