import { createContext, useContext } from "react";
import ActivityStore from "./activityStore";
import CommonStore from "./commonStore";
import UserStore from "./userStore";
import ModalStore from "./modalStore";
import ProfileStore from "./profileStore";
import CommentStore from "./commentStore";
import SubscriptionStore from "./subscriptionStore";
import RuleStore from "./ruleStore";
import RuleProjectStore from "./ruleProjectStore";
import DecisionTableStore from "./decisionTableStore";
import OrganizationStore from "./organizationStore";

interface Store {
    activityStore: ActivityStore;
    commonStore: CommonStore;
    userStore: UserStore;
    modalStore: ModalStore;
    profileStore: ProfileStore;
    commentStore: CommentStore;
    subscriptionStore: SubscriptionStore;
    ruleStore: RuleStore;
    ruleProjectStore: RuleProjectStore;
    decisionTableStore: DecisionTableStore;
    organizationStore: OrganizationStore;
}

export const store: Store = {
    activityStore: new ActivityStore(),
    commonStore: new CommonStore(),
    userStore: new UserStore(),
    modalStore: new ModalStore(),
    profileStore: new ProfileStore(),
    commentStore: new CommentStore(),
    subscriptionStore: new SubscriptionStore(),
    ruleStore: new RuleStore(),
    ruleProjectStore: new RuleProjectStore(),
    decisionTableStore: new DecisionTableStore(),
    organizationStore: new OrganizationStore()
}

export const StoreContext = createContext(store);

export function useStore() {
    return useContext(StoreContext);
}