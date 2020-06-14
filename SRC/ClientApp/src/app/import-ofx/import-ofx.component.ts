import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';

import { HubConnection } from '@aspnet/signalr';
import * as signalR from '@aspnet/signalr';

import { BlockUI, NgBlockUI } from 'ng-block-ui';

import { OFXService } from '../../services/ofx.service';

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

  public fileError: string = "";
  public totalError: number = 0;
  public totalSuccess: number = 0;

  constructor(
    private OFXService: OFXService,
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
      console.log(msg);
      this.blockUI.update(msg)
    });

    this._hubConnection.on('Sucess', (msg: string) => {
      //this.notification.success(msg);
      //this.modal.hide();
      console.log(msg);
      this.blockUI.stop();
    });

    this._hubConnection.on('Error', (msg: string) => {
      //this.notification.error(msg);
      //this.modal.hide();
      console.log(msg);
      this.blockUI.stop();
    });
  }

  clear() {
    this.file = null;
  }


  uploadFile(): void {
    this.blockUI.start("Enviando requisição.");
    this.OFXService.uploadFile(this.file).subscribe(
      result => {
        //this.totalSuccess = result.success;
        //this.totalError = result.error;
        //this.fileError = result.file;
      },
      error => {
        this.blockUI.stop();
        //            this.notification.error(error.error.message.slice(0, -1));
      },
      () => {
        if (this.totalError != 0) {
          //this.notification.error('Error upload file, ' + this.totalError + (this.totalError > 1 ? ' erros found.' : ' erro found.'));
          this.blockUI.stop();
        }
      }
    );
  }



  downloadFile(): void {
    this.OFXService.downloadFileError(this.fileError).subscribe(
      result => {
        var blob = new Blob([result], { type: 'text/csv' });
        var url = window.URL.createObjectURL(blob);
        var link = document.createElement("a");
        link.setAttribute("href", url);
        link.setAttribute("download", this.fileError);
        link.click();
      },
      error => {
        //this.notification.error("Erro no download.");
      },
      () => {
      }
    );
  }

  handleFileInput(files: FileList): void {
    this.file = files.item(0);
  }

}
