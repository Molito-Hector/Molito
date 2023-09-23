import { Action, Condition } from "./rule";

export interface DecisionTable {
    id: string;
    name: string;
    description: string;
    evaluationType: string;
    createdAt: any;
    conditions: Condition[];
    rows: DecisionRow[];
    ruleProjectId: string;
}

export interface DecisionRow {
    id?: string;
    tableId?: string;

    values: ConditionValue[];
    actions: Action[];
}

export interface ConditionValue {
    id?: string;
    decisionRowId?: string;
    conditionId: string;
    value: string;
}

export class DecisionTable implements DecisionTable {
    constructor(init?: DTFormValues) {
        Object.assign(this, init);
    }
}

export class DecisionRow implements DecisionRow {
    id?: string = undefined;
    values: ConditionValue[] = [{ conditionId: '', id: '', value: '' }];
    actions: Action[] = [{ id: '', name: '', targetProperty: '', modificationType: '', modificationValue: '' }];

    constructor(init?: DecisionRow) {
        Object.assign(this, init);
    }
}

export class DTFormValues {
    id?: string = undefined;
    name: string = '';
    description: string = '';
    evaluationType: string = '';
    conditions: Condition[] = [];
    rows: DecisionRow[] = [];
    ruleProjectId: string = '';

    constructor(table?: DTFormValues) {
        if (table) {
            this.id = table.id;
            this.name = table.name;
            this.description = table.description;
            this.evaluationType = table.evaluationType;
            this.conditions = table.conditions;
            this.rows = table.rows;
            this.ruleProjectId = table.ruleProjectId;
        }
    }
}