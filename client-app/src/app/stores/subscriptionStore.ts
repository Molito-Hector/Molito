import { makeAutoObservable, runInAction } from "mobx";
import { Subscription } from "../models/subscription";

export default class SubscriptionStore {
    subscription: Subscription | null = null;
    loading = true;
    subFound = false;

    constructor() {
        makeAutoObservable(this);
    }

    getSubscriptionData = async (encodedURL: string) => {
        try {
            const decodedURL = decodeURI(encodedURL);
            const response = await fetch("https://monastoragen5chkhjo2y2s2.blob.core.windows.net" + decodedURL);
            const data = await response.json();
            this.subscription = data;
            runInAction(() => {
                this.subFound = true;
                this.loading = false;
            });
        } catch (error) {
            console.log("Failed to fetch subscription data: " + error);
            runInAction(() => this.loading = false);
        }
    }
}