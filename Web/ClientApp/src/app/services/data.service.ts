import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class DataService {

    private url = "/api/account";

    constructor(private http: HttpClient) {
    }

    getAccount(accountId: string) {
        return this.http.get(this.url + '?accountId=' + accountId);
    }
}