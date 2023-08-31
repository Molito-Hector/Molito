export interface Rule {
    id: string;
    name: string;
    description: string;
    createdAt: any;
    conditions: Condition[];
    type?: string;
    [key: string]: any;
}

export interface Condition {
    id?: string;
    field: string;
    operator: string;
    value: string;
    logicalOperator?: string;
    subConditions?: Condition[];
    actions: Action[];
}

export interface Action {
    id?: string;
    name: string;
    targetProperty: string;
    modificationType: string;
    modificationValue: string;
}

export class Rule implements Rule {
    constructor(init?: RuleFormValues) {
        Object.assign(this, init);
    }
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