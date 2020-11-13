import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { AppComponent } from './app.component';
import { MainPageComponent } from './main-page/main-page.component';
import { ServicePageComponent } from './service-page/service-page.component';
import { AppFooterComponent } from './app-footer/app-footer.component';
import { RouterModule } from '@angular/router';

@NgModule({
    imports: [BrowserModule, FormsModule, HttpClientModule, RouterModule.forRoot([], { anchorScrolling: 'enabled' })],
    declarations: [AppComponent, MainPageComponent, ServicePageComponent, AppFooterComponent],
    bootstrap: [AppComponent]
})
export class AppModule { }