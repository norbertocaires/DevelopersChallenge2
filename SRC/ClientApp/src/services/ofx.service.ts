import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Observable, of } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';

import { TransactionsViewModel } from '../models/ofx/TransactionsViewModel';

const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
  responseType: 'text' as 'json'
};

@Injectable()
export class OFXService {

  constructor(private http: HttpClient) { }
  uploadFile(file: File | null) {
    let input = new FormData();
    input.append("file", file ? file : "");
    return this.http.post('ofx/uploadfile', input);
  }

  listTransactionsPaged(page: number, rows: number): Observable<TransactionsViewModel[]> {
    var options = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
      params: {
        'page': String(page),
        'rows': String(rows)
      }
    };
    return this.http.get<TransactionsViewModel[]>('ofx/listtransactionspaged', options);
  }

  getTotalTransactions(): Observable<number> {
    return this.http.get<number>('ofx/gettotaltransactions');
  }
}
