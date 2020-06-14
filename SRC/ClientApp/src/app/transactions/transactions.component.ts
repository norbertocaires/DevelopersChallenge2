import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';

import { BlockUI, NgBlockUI } from 'ng-block-ui';

import { OFXService } from '../../services/ofx.service';
import { TransactionsViewModel } from '../../models/ofx/TransactionsViewModel';

@Component({
  selector: 'app-transactions',
  templateUrl: './transactions.component.html',
  styleUrls: ['./transactions.component.css']
})
export class TransactionsComponent {

  @BlockUI() blockUI: NgBlockUI;
  public transactions: Array<TransactionsViewModel> = [];

  public totalTransaction: number;
  public currentPage: number = 1;
  public perPage: number = 8;


  constructor(
    private OFXService: OFXService,
  ) {
    this.getTotalTransactions();
  }

  getTotalTransactions(): void {
    this.blockUI.start("Send Request");
    this.OFXService.getTotalTransactions().subscribe(
      result => {
        this.totalTransaction = result;
        this.listTransactionsPaged(this.currentPage);
        this.blockUI.stop();
      },
      error => {
        this.blockUI.stop();
      },
    );
  }


  listTransactionsPaged(currentPage: number): void {
    this.currentPage = currentPage;
    this.blockUI.start("Send Request");
    this.OFXService.listTransactionsPaged(this.currentPage, this.perPage).subscribe(
      result => {
        this.transactions = result;
        this.blockUI.stop();
      },
      error => {
        this.blockUI.stop();
      },
    );
  }
}
