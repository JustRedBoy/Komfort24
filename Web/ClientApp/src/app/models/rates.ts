export class Rates {
    constructor(
        public specialWerRate: number,
        public generalWerRate: number,
        public waterRate: number,
        public centralHeatingRate: number,
        public customHeatingRate: number,
        public centralHeatingForAllRate: number, // Temporarily unused
        public centralHeatingForSomeRate: number, // Temporarily unused
        public garbageRate: number,
        public repairRate: number) { }
}