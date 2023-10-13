import { makeAutoObservable, runInAction } from "mobx";
import agent from "../api/agent";
import { Pagination, PagingParams } from "../models/pagination";
import { Profile } from "../models/profile";
import { OrgFormValues, Organization } from "../models/organization";

export default class OrganizationStore {
    organizationRegistry = new Map<string, Organization>();
    selectedOrganization: Organization | undefined = undefined;
    editMode = false;
    loading = false;
    loadingInitial = false;
    pagination: Pagination | null = null;
    pagingParams = new PagingParams();
    predicate = new Map().set('all', true);

    constructor() {
        makeAutoObservable(this);
    }

    loadOrganization = async (id: string) => {
        console.log("Loading: " + id);
        let organization = this.getOrganization(id);
        if (organization && organization.name !== undefined) {
            this.selectedOrganization = organization;
            return organization;
        }
        else {
            this.setLoadingInitial(true);
            try {
                organization = await agent.Organizations.details(id);
                this.setOrganization(organization);
                runInAction(() => this.selectedOrganization = organization);
                this.setLoadingInitial(false);
                return organization;
            } catch (error) {
                console.log(error);
                this.setLoadingInitial(false);
            }
        }
    }

    private setOrganization = (organization: Organization) => {
        this.organizationRegistry.set(organization.id, organization);
    }

    private getOrganization = (id: string) => {
        return this.organizationRegistry.get(id);
    }

    setLoadingInitial = (state: boolean) => {
        this.loadingInitial = state;
    }

    setLoading = (state: boolean) => {
        this.loading = state;
    }

    createOrganization = async (organization: OrgFormValues) => {
        try {
            const organizationToCreate = {
                ...organization
            };
            await agent.Organizations.create(organizationToCreate);
            const newOrganization = new Organization(organizationToCreate);
            this.setOrganization(newOrganization);
            runInAction(() => {
                this.selectedOrganization = newOrganization;
            })
        } catch (error) {
            console.log(error);
        }
    }

    updateMembership = async (username: string) => {
        const user = (await agent.Account.getUser(username)).data;
        this.loading = true;
        try {
            await agent.Organizations.updateMember(this.selectedOrganization!.id, username);
            runInAction(() => {
                if (this.selectedOrganization?.members.some(x => x.username === username)) {
                    this.selectedOrganization.members = this.selectedOrganization.members?.filter(a => a.username !== username);
                } else {
                    const member = new Profile(user!);
                    this.selectedOrganization?.members?.push(member);
                }
                this.organizationRegistry.set(this.selectedOrganization!.id, this.selectedOrganization!)
            })
        } catch (error) {
            console.log(error);
        } finally {
            runInAction(() => this.loading = false);
        }
    }

    updateOrg = async (org: OrgFormValues) => {
        this.loading = true;
        const id = this.selectedOrganization?.id;
        try {
            await agent.Organizations.edit(id!, org);
            runInAction(() => {
                if (org.id) {
                    let updatedOrg = { ...this.getOrganization(org.id), ...org }
                    this.organizationRegistry.set(org.id, updatedOrg as Organization);
                    this.selectedOrganization = updatedOrg as Organization;
                }
            })
        } catch (error) {
            console.log(error);
        } finally {
            runInAction(() => this.loading = false);
        }
    }

    // deleteRuleProject = async (id: string) => {
    //     this.loading = true;
    //     try {
    //         await agent.RuleProjects.delete(id);
    //         runInAction(() => {
    //             this.ruleProjectRegistry.delete(id);
    //             this.loading = false;
    //         })
    //     } catch (error) {
    //         console.log(error);
    //         this.setLoading(false);
    //     }
    // }

    clearSelectedOrganization = () => {
        this.selectedOrganization = undefined;
    }
}