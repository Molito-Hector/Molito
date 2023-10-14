export interface IRule {
    id: string;
    name: string;
    description: string;
    createdAt: Date;
    conditions: Condition[];
    type?: string;
    [key: string]: Date | Condition[] | string | undefined;
}

export interface Condition {
    id?: string;
    field: string;
    operator: string;
    value: string;
    logicalOperator?: string;
    subConditions?: Condition[];
    tableColumnIndex?: number;
    actions: Action[];
}

export interface Action {
    id?: string;
    tableId?: string;
    name: string;
    targetProperty: string;
    modificationType: string;
    modificationValue: string;
}

export class Rule implements IRule {
    constructor(init: RuleFormValues) {
        this.id = init.id!
        this.name = init.name
        this.description = init.description
    }

    id: string;
    name: string;
    description: string;
    createdAt: Date = new Date();
    conditions: Condition[] = [];
    type?: string;
    [key: string]: Date | Condition[] | string | undefined;
}

export class RuleFormValues {
    id?: string = undefined;
    name: string = '';
    description: string = '';
    ruleProjectId: string = '';

    constructor(rule?: RuleFormValues) {
        if (rule) {
            this.id = rule.id;
            this.name = rule.name;
        }
    }
}