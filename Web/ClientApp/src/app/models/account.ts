import { House } from './house';
import { Rates } from './rates';
import { Report } from './report';

export class Account {
    constructor(
        public accountId: string = "",
        public flatNumber: string = "",
        public owner: string = "",
        public house: House = new House("", new Rates(0, 0, 0, 0, 0, 0, 0, 0, 0)),
        public currentReport: Report = new Report(0, 0, 0, "", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)) { }
}
