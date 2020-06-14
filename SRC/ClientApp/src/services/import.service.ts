import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Observable, of } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';

import { ImportViewModel } from '../models/import/ImportViewModel';

const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
  responseType: 'text' as 'json'
};

@Injectable()
export class ImportsService {

  constructor(private http: HttpClient) { }


  listTransactionsPaged(page: number, rows: number): Observable<ImportViewModel[]> {
    var options = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
      params: {
        'page': String(page),
        'rows': String(rows)
      }
    };
    return this.http.get<ImportViewModel[]>('import/listimportspaged', options);
  }

  getTotalImports(): Observable<number> {
    return this.http.get<number>('import/gettotalimports');
  }

  downloadFile(fileName: string): Observable<File> {
    var options = {
      params: { 'fileName': fileName },
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
      responseType: 'text' as 'json'
    };
    return this.http.get<File>('import/downloadfile', options);
  }

  downloadFileDuplicates(fileName: string): Observable<File> {
    var options = {
      params: { 'fileName': fileName },
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
      responseType: 'text' as 'json'
    };
    return this.http.get<File>('import/downloadfileduplicate', options);
  }
}
