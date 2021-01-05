import { Component, ElementRef, ViewChild } from '@angular/core';
import { DataService } from '../services/data.service';
import { Account } from '../models/account';

@Component({
    selector: 'service-page',
    styleUrls: ['./service-page.component.css'],
    templateUrl: './service-page.component.html',
    providers: [DataService]
})
export class ServicePageComponent {
    accountId: string = "";
    accountFound: boolean = true;
    accountShow: boolean = false;
    account: Account = new Account();

    @ViewChild('accId') accountIdSearch: ElementRef;

    constructor(private dataService: DataService) { }

    loadAccount() {
        if (this.accountId.match(/(^\d{4}$)|(^\d{4}\/[1|2]$)/)) {
            this.accountIdSearch.nativeElement.blur();
            this.dataService.getAccount(this.accountId).subscribe(data => {
                if (data == null) {
                    this.account = new Account();
                    this.accountFound = false;
                    this.accountShow = false;
                }
                else {
                    this.account = data;
                    this.accountFound = true;
                    this.accountShow = true;
                    this.updateTotal();
                }
            });
        }
        else {
            if (this.accountId.length >= 4) {
                this.account = new Account();
                this.accountFound = false;
            }
            else {
                this.accountFound = true;
            }
            this.accountShow = false;
        }
    }

    updateTotal() {
        var total = 0;
        if (this.account.accountId != "") {
            if (this.account.currentReport.werEndState > 0) {
                total += this.account.currentReport.werEndState;
            }

            if (this.isNumber(this.account.currentReport.waterCurrentValue)) {
                this.account.currentReport.waterForMonth = (this.account.currentReport.waterCurrentValue - this.account.currentReport.waterPreviousValue) * this.account.house.rates.waterRate;
                this.account.currentReport.waterEndState = this.account.currentReport.waterForMonth - this.account.currentReport.waterPaid;
                if (this.account.currentReport.waterEndState > 0) {
                    total += this.account.currentReport.waterEndState;
                }
            }

            if (this.account.currentReport.heatingType != "") {
                if (this.isNumber(this.account.currentReport.heatingCurrentValue)) {
                    var coef = 0.0;
                    switch (this.account.currentReport.heatingType.toLowerCase()) {
                        case "гкал":
                            coef = 1.1;
                            break;
                        case "мвт":
                            coef = 0.86 * 1.1;
                            break;
                        case "квт":
                            coef = 1.1 / 1162.2;
                            break;
                        case "гдж":
                            coef = 1.1 / 4.187;
                            break;
                    }
                    this.account.currentReport.heatingForService = (this.account.currentReport.heatingCurrentValue - this.account.currentReport.heatingPreviousValue) * this.account.house.rates.customHeatingRate * coef;

                    if (this.account.currentReport.heatingPaid >= 0) {
                        this.account.currentReport.heatingEndState = this.account.currentReport.heatingStartState + this.account.currentReport.heatingForService - this.account.currentReport.heatingPaid;
                    }
                    else {
                        this.account.currentReport.heatingEndState = this.account.currentReport.heatingStartState + this.account.currentReport.heatingForService - this.account.currentReport.heatingPreviliges;
                    }
                    if (this.account.currentReport.heatingEndState > 0) {
                        total += this.account.currentReport.heatingEndState;
                    }
                }
            }
            else {
                if (this.account.currentReport.heatingEndState > 0) {
                    total += this.account.currentReport.heatingEndState;
                }
            }
            this.account.currentReport.total = total;
        }
    }

    isNumber(n) {
        return !isNaN(parseFloat(n)) && !isNaN(n - 0);
    }
}