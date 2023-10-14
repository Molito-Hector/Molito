import { Action, Condition } from "./rule";

export interface IDecisionTable {
    id: string;
    name: string;
    description: string;
    evaluationType: string;
    createdAt: Date;
    conditions: Condition[];
    actions: Action[];
    rows: DecisionRow[];
    ruleProjectId: string;
}

export interface IDecisionRow {
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

export class DecisionTable implements IDecisionTable {
    constructor(init: DTFormValues) {
        this.id = init.id!
        this.actions = init.actions
        this.conditions = init.conditions
        this.description = init.description
        this.evaluationType = init.evaluationType
        this.name = init.name
        this.rows = init.rows
        this.ruleProjectId = init.ruleProjectId
    }

    id: string;
    name: string;
    description: string;
    evaluationType: string;
    createdAt: Date = new Date();
    conditions: Condition[];
    actions: Action[];
    rows: DecisionRow[];
    ruleProjectId: string;
}

export class DecisionRow implements IDecisionRow {
    id?: string = undefined;
    tableId?: string = undefined;
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