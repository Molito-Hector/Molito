export interface Subscription {
    subscriptionId: string;
    subscriptionName: string;
    offerId: string;
    planId: string;
    seatQuantity: number;
    status: number;
    term: Date;
    beneficiary: AzureUser;
    purchaser: AzureUser;
}

export interface AzureUser {
    userId: string;
    userEmail: string;
    aadObjectId: string;
    aadTenantId: string;
}