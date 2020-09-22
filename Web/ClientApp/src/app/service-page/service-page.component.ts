import { Component } from '@angular/core';
import { DataService } from '../services/data.service';
import { Account } from '../models/account';

@Component({
    selector: 'service-page',
    styleUrls: ['./service-page.component.css'],
    templateUrl: './service-page.component.html',
    providers: [DataService]
})
export class ServicePage {
    accountId: string = "";
    accountFound: boolean = true;
    accountShow: boolean = false;
    account: Account = new Account();

    constructor(private dataService: DataService) { }

    loadAccount() {
        if (this.accountId.match(/(^\d{4}$)|(^\d{4}\/[1|2]$)/)) {
            this.dataService.getAccount(this.accountId).subscribe((data: Account) => {
                if (data == null) {
                    this.account = new Account();
                    this.accountFound = false;
                    this.accountShow = false;
                }
                else {
                    this.account = data;
                    this.accountFound = true;
                    this.accountShow = true;
                }
            });
        }
        else {
            this.account = new Account();
            this.accountShow = false;
        }
    }
}