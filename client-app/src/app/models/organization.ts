import { Profile } from "./profile";

export interface Organization {
    id: string;
    name: string;
    description: string;
    members: Profile[];
}

export class Organization implements Organization {
    constructor(init?: OrgFormValues) {
        Object.assign(this, init);
    }
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