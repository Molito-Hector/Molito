import { Profile } from "./profile";

export interface IOrganization {
    id: string;
    name: string;
    description: string;
    members: Profile[];
}

export class Organization implements IOrganization {
    constructor(init: OrgFormValues) {
        this.id = init.id!
        this.name = init.name
        this.description = init.description
    }

    id: string;
    name: string;
    description: string;
    members: Profile[] = [];
}

export class OrgFormValues {
    id?: string = undefined;
    name: string = '';
    description: string = '';

    constructor(org?: OrgFormValues) {
        if (org) {
            this.id = org.id;
            this.name = org.name;
            this.description = org.description;
        }
    }
}