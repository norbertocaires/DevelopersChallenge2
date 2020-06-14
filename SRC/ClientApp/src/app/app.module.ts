import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { BlockUIModule } from 'ng-block-ui';
import { NgxPaginationModule } from 'ngx-pagination';

//COMPONENTS
import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { ImportOfxComponent } from './import-ofx/import-ofx.component';
import { TransactionsComponent } from './transactions/transactions.component';
import { ImportsComponent } from './imports/imports.component'

//SERVICES
import { OFXService } from '../services/ofx.service';
import { ImportsService } from '../services/import.service';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    ImportOfxComponent,
    TransactionsComponent,
    ImportsComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    NgxPaginationModule,
    BlockUIModule.forRoot(),
    RouterModule.forRoot([
      { path: '', component: TransactionsComponent, pathMatch: 'full' },
      { path: 'import', component: ImportOfxComponent },
      { path: 'imports', component: ImportsComponent },
      { path: '**', redirectTo: '' }
    ])
  ],
  providers: [
    OFXService,
    ImportsService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
