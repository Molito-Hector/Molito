import { DecisionTable } from "./decisionTable";
import { Profile } from "./profile";
import { Rule } from "./rule";

export interface RuleProject {
    id: string;
    name: string;
    description: string;
    createdAt: any;
    properties: RuleProperty[];
    standardRules: Rule[];
    decisionTables: DecisionTable[];
    members: Profile[];
}

export interface RuleProperty {
    id: string;
    name: string;
    type: string;
    direction: string;
    subProperties: RuleProperty[];
}

export class RuleProject implements RuleProject {
    constructor(init?: RPFormValues) {
        Object.assign(this, init);
    }
}

export class RPFormValues {
    id?: string = undefined;
    name: string = '';
    description: string = '';

    constructor(table?: RPFormValues) {
        if (table) {
            this.id = table.id;
            this.name = table.name;
            this.description = table.description;
        }
    }
}