import { DecisionTable } from "./decisionTable";
import { Profile } from "./profile";
import { Rule } from "./rule";

export interface IRuleProject {
    id: string;
    name: string;
    description: string;
    createdAt: Date;
    properties: RuleProperty[];
    standardRules: Rule[];
    decisionTables: DecisionTable[];
    members: Profile[];
    owner: string;
}

export interface RuleProperty {
    id: string;
    name: string;
    type: string;
    direction: string;
    subProperties: RuleProperty[];
}

export class RuleProject implements IRuleProject {
    constructor(init: RPFormValues) {
        this.id = init.id!
        this.name = init.name
        this.description = init.description
    }

    id: string;
    name: string;
    description: string;
    createdAt: Date = new Date();
    properties: RuleProperty[] = [];
    standardRules: Rule[] = [];
    decisionTables: DecisionTable[] = [];
    members: Profile[] = [];
    owner: string = '';
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