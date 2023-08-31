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
    values: ConditionValue[];
    actions: Action[];
}

export interface ConditionValue {
    id: string;
    conditionId: string;
    value: string;
}

export class DecisionTable implements DecisionTable {
    constructor(init?: DTFormValues) {
        Object.assign(this, init);
    }
}

export class DTFormValues {
    id?: string = undefined;
    name: string = '';
    description: string = '';
    evaluationType: string = '';
    conditions: Condition[] = [{ field: '', operator: '', value: '', logicalOperator: '', subConditions: [], actions: [] }];
    rows: DecisionRow[] = [{ values: [], actions: [] }];
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