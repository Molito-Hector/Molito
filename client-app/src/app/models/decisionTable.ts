import { Action, Condition } from "./rule";

export interface DecisionTable {
    id: string;
    name: string;
    description: string;
    evaluationType: string;
    createdAt: any;
    conditions: Condition[];
    actions: Action[];
    rows: DecisionRow[];
    ruleProjectId: string;
}

export interface DecisionRow {
    id?: string;
    tableId?: string;
    values: ConditionValue[];
    actionValues: ActionValue[];
}

export interface ConditionValue {
    id?: string;
    decisionRowId?: string;
    conditionId: string;
    value: string;
}

export interface ActionValue {
    id?: string;
    decisionRowId?: string;
    actionId: string;
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
    actionValues: ActionValue[] = [{ id: '', actionId: '', value: '' }];

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
    actions: Action[] = [];
    rows: DecisionRow[] = [];
    ruleProjectId: string = '';

    constructor(table?: DTFormValues) {
        if (table) {
            this.id = table.id;
            this.name = table.name;
            this.description = table.description;
            this.evaluationType = table.evaluationType;
            this.conditions = table.conditions;
            this.actions = table.actions;
            this.rows = table.rows;
            this.ruleProjectId = table.ruleProjectId;
        }
    }
}