import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';

import { HubConnection } from '@aspnet/signalr';
import * as signalR from '@aspnet/signalr';

import { BlockUI, NgBlockUI } from 'ng-block-ui';

import { OFXService } from '../../services/ofx.service';
import { ImportsService } from '../../services/import.service';
import { ImportViewModel } from '../../models/import/ImportViewModel';

@Component({
  selector: 'app-import-ofx',
  templateUrl: './import-ofx.component.html',
  styleUrls: ['./import-ofx.component.css']
})
export class ImportOfxComponent {
  @ViewChild('uploadInput') uploadInput: ElementRef;
  private _hubConnection: HubConnection | undefined;


  @BlockUI() blockUI: NgBlockUI;

  public file: File | null;
  public error: boolean = false;
  public import: ImportViewModel = null;

  constructor(
    private OFXService: OFXService,
    private ImportsService: ImportsService,
  ) {
    this._hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('/notify')
      .configureLogging(signalR.LogLevel.Information)
      .build();

    this._hubConnection
      .start()
      .then(() => console.log('connection started!'))
      .catch(err => console.log('error while establishing connection :('));

    this._hubConnection.on('updatepercent', (msg: string) => {
      this.blockUI.update(msg)
    });

    this._hubConnection.on('Sucess', (response: ImportViewModel) => {
      this.import = response;
      console.log(response);
      this.blockUI.stop();
    });

    this._hubConnection.on('Error', (msg: string) => {
      this.error = true;
      this.blockUI.stop();
    });
  }

  clear() {
    this.file = null;
    this.import = null;
  }


  uploadFile(): void {
    this.blockUI.start("Send Request.");
    this.OFXService.uploadFile(this.file).subscribe(
      result => {
      },
      error => {
        this.error = true;
        this.blockUI.stop();
      },
    );
  }

  handleFileInput(files: FileList): void {
    this.file = files.item(0);
  }

  downloadFile(file: string, type: string): void {
    this.ImportsService.downloadFile(file).subscribe(
      result => {
        var blob = new Blob([result], { type: 'text/ofx' });
        var url = window.URL.createObjectURL(blob);
        var link = document.createElement("a");
        link.setAttribute("href", url);
        link.setAttribute("download", file);
        link.click();
      },
      error => {
      },
      () => {
      }
    );
  }


  downloadFileDuplicate(file: string): void {
    this.ImportsService.downloadFileDuplicates(file).subscribe(
      result => {
        var blob = new Blob([result], { type: 'text/csv' });
        var url = window.URL.createObjectURL(blob);
        var link = document.createElement("a");
        link.setAttribute("href", url);
        link.setAttribute("download", file);
        link.click();
      },
      error => {
      },
      () => {
      }
    );
  }

}
