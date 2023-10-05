export interface Organization {
    name: string;
    description: string;
    roles: string[];
    token: string;
    image?: string;
}

export interface OrgFormValues {
    email: string;
    password: string;
    displayName?: string;
    username?: string;
}