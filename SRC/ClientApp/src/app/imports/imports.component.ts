import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';

import { BlockUI, NgBlockUI } from 'ng-block-ui';

import { ImportsService } from '../../services/import.service';
import { ImportViewModel } from '../../models/import/ImportViewModel';

@Component({
  selector: 'app-imports',
  templateUrl: './imports.component.html',
  styleUrls: ['./imports.component.css']
})
export class ImportsComponent {

  @BlockUI() blockUI: NgBlockUI;
  public imports: Array<ImportViewModel> = [];

  public totalImports: number;
  public currentPage: number = 1;
  public perPage: number = 8;


  constructor(
    private ImportsService: ImportsService,
  ) {
    this.getTotalImports();
  }

  getTotalImports(): void {
    this.blockUI.start("Send Request");
    this.ImportsService.getTotalImports().subscribe(
      result => {
        this.totalImports = result;
        this.listImportsPaged(this.currentPage);
        this.blockUI.stop();
      },
      error => {
        this.blockUI.stop();
      },
    );
  }


  listImportsPaged(currentPage: number): void {
    this.currentPage = currentPage;
    this.blockUI.start("Send Request");
    this.ImportsService.listTransactionsPaged(this.currentPage, this.perPage).subscribe(
      result => {
        this.imports = result;
        this.blockUI.stop();
      },
      error => {
        this.blockUI.stop();
      },
    );
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
