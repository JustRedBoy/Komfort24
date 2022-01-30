export class Report {
    public heatingStartState: number = 0;
    public heatingFixedPreviousValue: number = 0;
    public heatingPaid: number = 0;
    public heatingEndState: number = 0;
    public werStartState: number = 0;
    public werPaid: number = 0;
    public werEndState: number = 0;
    public waterFixedPreviousValue: number = 0;
    public waterPaid: number = 0;
    public waterEndState: number = 0;
    public total: number = 0;

    constructor(
        public heatingStartDebit: number,
        public heatingStartCredit: number,
        public heatingSquare: number,
        public heatingType: string,
        public heatingCurrentValue: number,
        public heatingPreviousValue: number,
        public heatingValue: number,
        public heatingForService: number,
        public heatingPreviliges: number,
        public heatingTotal: number,
        public heatingBank: number,
        public heatingCash: number,
        public heatingEndDebit: number,
        public heatingEndCredit: number,

        public werStartDebit: number,
        public werStartCredit: number,
        public werSquare: number,
        public werForMonth: number,
        public repairForMonth: number,
        public livingPersons: number,
        public garbageForMonth: number,
        public waterCurrentValue: number,
        public waterPreviousValue: number,
        public waterValue: number,
        public waterForMonth: number,
        public werWaterForService: number,
        public werPreviliges: number,
        public werTotal: number,
        public werRepair: number,
        public werBank: number,
        public werCash: number,
        public werEndDebit: number,
        public werEndCredit: number) { }
}