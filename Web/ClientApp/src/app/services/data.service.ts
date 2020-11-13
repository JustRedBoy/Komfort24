import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Account } from '../models/account';
import { map } from 'rxjs/operators';

@Injectable()
export class DataService {

    private url = "/api/account";

    constructor(private http: HttpClient) {
    }

    getAccount(accountId: string) {
        return this.http.get<Account>(this.url + '?accountId=' + accountId).pipe(
            map(account => {
                if (account != null) {
                    let currentReport = account.currentReport;
                    currentReport.heatingStartState = currentReport.heatingStartDebit - currentReport.heatingStartCredit;
                    currentReport.heatingFixedPreviousValue = currentReport.heatingPreviousValue + currentReport.heatingValue;
                    currentReport.heatingPaid = currentReport.heatingBank + currentReport.heatingCash;
                    currentReport.heatingEndState = currentReport.heatingStartState + currentReport.heatingForService - currentReport.heatingPreviliges - currentReport.heatingPaid;

                    currentReport.waterFixedPreviousValue = currentReport.waterPreviousValue + currentReport.waterValue;
                    currentReport.waterPaid = currentReport.waterValue * account.house.rates.waterRate;
                    currentReport.waterEndState = 0;

                    currentReport.werStartState = currentReport.werStartDebit - currentReport.werStartCredit;
                    currentReport.werPaid = currentReport.werBank + currentReport.heatingCash - currentReport.waterPaid;
                    currentReport.werEndState = currentReport.werEndDebit - currentReport.werEndCredit;

                    currentReport.total = currentReport.werEndState + currentReport.heatingEndState + currentReport.waterEndState;
                    return account;
                }
                return null;
        }));
    }
}