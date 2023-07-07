export interface Rule {
    id: string;
    name: string;
    conditions: Condition[];
    actions: Action[];
}

export interface Condition {
    id?: string;
    field: string;
    operator: string;
    value: string;
}

export interface Action {
    id?: string;
    name: string;
}

export class Rule implements Rule {
    constructor(init?: RuleFormValues) {
        Object.assign(this, init);
    }
}

export class RuleFormValues {
    id?: string = undefined;
    name: string = '';
    conditions: Condition[] = [];
    actions: Action[] = [];

    constructor(rule?: RuleFormValues) {
        if (rule) {
            this.id = rule.id;
            this.name = rule.name;
            this.conditions = rule.conditions;
            this.actions = rule.actions;
        }
    }
}