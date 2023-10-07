export interface User {
    username: string;
    displayName: string;
    roles: string[];
    token: string;
    image?: string;
    orgId?: string;
}

export interface UserFormValues {
    email: string;
    password: string;
    displayName?: string;
    username?: string;
}