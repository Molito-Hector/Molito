import { makeAutoObservable, reaction, runInAction } from "mobx";
import agent from "../api/agent";
import { Pagination, PagingParams } from "../models/pagination";
import { RPFormValues, RuleProject, RuleProperty } from "../models/ruleProject";
import { DecisionTable } from "../models/decisionTable";
import { Rule } from "../models/rule";
import { store } from "./store";
import { Profile } from "../models/profile";
import { User } from "../models/user";

export default class RuleProjectStore {
    ruleProjectRegistry = new Map<string, RuleProject>();
    selectedRuleProject: RuleProject | undefined = undefined;
    editMode = false;
    loading = false;
    loadingInitial = false;
    pagination: Pagination | null = null;
    pagingParams = new PagingParams();
    predicate = new Map().set('all', true);

    constructor() {
        makeAutoObservable(this);

        reaction(
            () => this.predicate.keys(),
            () => {
                this.pagingParams = new PagingParams();
                this.ruleProjectRegistry.clear();
                this.loadRuleProjects();
            }
        )
    }

    add(item: DecisionTable | Rule) {
        if (item instanceof DecisionTable) {
            this.selectedRuleProject!.decisionTables.push(item);
        } else if (item instanceof Rule) {
            this.selectedRuleProject!.standardRules.push(item);
        }
    }

    remove(id: string) {
        const decisionTableIndex = this.selectedRuleProject!.decisionTables.findIndex(dt => dt.id === id);
        if (decisionTableIndex !== -1) {
            this.selectedRuleProject!.decisionTables.splice(decisionTableIndex, 1);
            return;
        }

        const ruleIndex = this.selectedRuleProject!.standardRules.findIndex(r => r.id === id);
        if (ruleIndex !== -1) {
            this.selectedRuleProject!.standardRules.splice(ruleIndex, 1);
            return;
        }
    }

    setPagingParams = (pagingParams: PagingParams) => {
        this.pagingParams = pagingParams;
    }

    setPredicate = (predicate: string, value: string | Date) => {
        const resetPredicate = () => {
            this.predicate.forEach((value, key) => {
                if (key !== 'startDate') this.predicate.delete(key);
            })
        }
        switch (predicate) {
            case 'all':
                resetPredicate();
                this.predicate.set('all', true);
                break;
            case 'isGoing':
                resetPredicate();
                this.predicate.set('isGoing', true);
                break;
            case 'isHost':
                resetPredicate();
                this.predicate.set('isHost', true);
                break;
            case 'startDate':
                this.predicate.delete('startDate');
                this.predicate.set('startDate', value);
        }
    }

    get axiosParams() {
        const params = new URLSearchParams();
        params.append('pageNumber', this.pagingParams.pageNumber.toString());
        params.append('pageSize', this.pagingParams.pageSize.toString());
        this.predicate.forEach((value, key) => {
            if (key === 'startDate') {
                params.append(key, (value as Date).toISOString());
            } else {
                params.append(key, value);
            }
        })
        return params;
    }

    get ruleProjectsByName() {
        runInAction(() => {
            this.ruleProjectRegistry.forEach(ruleProject => {
                ruleProject.createdAt = new Date(ruleProject.createdAt);
            })
        });
        return Array.from(this.ruleProjectRegistry.values()).sort((a, b) => {
            const nameA = a.name.toUpperCase();
            const nameB = b.name.toUpperCase();
            if (nameA < nameB) {
                return -1;
            }
            if (nameA > nameB) {
                return 1;
            }
            return 0;
        });
    }

    get groupedRuleProjects() {
        return Object.entries(
            this.ruleProjectsByName.reduce((ruleprojects, ruleproject) => {
                const name = ruleproject.name;
                ruleprojects[name] = ruleprojects[name] ? [...ruleprojects[name], ruleproject] : [ruleproject];
                return ruleprojects;
            }, {} as { [key: string]: RuleProject[] })
        )
    }

    loadRuleProjects = async () => {
        this.setLoadingInitial(true);
        try {
            const result = await agent.RuleProjects.list(this.axiosParams);
            result.data.forEach(ruleproject => {
                this.setRuleProject(ruleproject);
            });
            this.setPagination(result.pagination);
            this.setLoadingInitial(false);
        } catch (error) {
            console.log(error);
            this.setLoadingInitial(false);
        }
    }

    setPagination = (pagination: Pagination) => {
        this.pagination = pagination;
    }

    loadRuleProject = async (id: string) => {
        let ruleProject = this.getRuleProject(id);
        if (ruleProject && ruleProject.properties !== undefined) {
            this.selectedRuleProject = ruleProject;
            return ruleProject;
        }
        else {
            this.setLoadingInitial(true);
            try {
                ruleProject = await agent.RuleProjects.details(id);
                ruleProject.createdAt = new Date(ruleProject.createdAt);
                runInAction(() => {
                    ruleProject?.standardRules.forEach(rule => {
                        rule.createdAt = new Date(rule.createdAt);
                    });
                    ruleProject?.decisionTables.forEach(table => {
                        table.createdAt = new Date(table.createdAt);
                    })
                    ruleProject?.properties.forEach(property => {
                        if (property.subProperties.length > 0) {
                            property.subProperties.forEach(subProperty => {
                                subProperty.direction = property.direction;
                            })
                        }
                    })
                });
                this.setRuleProject(ruleProject);
                runInAction(() => this.selectedRuleProject = ruleProject);
                this.setLoadingInitial(false);
                return ruleProject;
            } catch (error) {
                console.log(error);
                this.setLoadingInitial(false);
            }
        }
    }

    private setRuleProject = (ruleproject: RuleProject) => {
        this.ruleProjectRegistry.set(ruleproject.id, ruleproject);
    }

    private getRuleProject = (id: string) => {
        return this.ruleProjectRegistry.get(id);
    }

    setLoadingInitial = (state: boolean) => {
        this.loadingInitial = state;
    }

    setLoading = (state: boolean) => {
        this.loading = state;
    }

    createRuleProject = async (ruleProject: RPFormValues) => {
        try {
            const ruleProjectToCreate = {
                ...ruleProject
            };
            await agent.RuleProjects.create(ruleProjectToCreate);
            const newRuleProject = new RuleProject(ruleProjectToCreate);
            newRuleProject.createdAt = new Date();
            this.setRuleProject(newRuleProject);
            runInAction(() => {
                this.selectedRuleProject = newRuleProject;
            })
        } catch (error) {
            console.log(error);
        }
    }

    addProperties = async (properties: RuleProperty[]) => {
        this.loading = true;
        const id = this.selectedRuleProject!.id;
        try {
            await agent.RuleProjects.addProperties(id, properties);
            runInAction(() => {
                properties.forEach(property => {
                    if (property.subProperties.length > 0) {
                        property.subProperties.forEach(subProperty => {
                            subProperty.direction = property.direction;
                        })
                    }
                    this.selectedRuleProject?.properties.push(property);
                });
            })
            this.ruleProjectRegistry.set(this.selectedRuleProject!.id, this.selectedRuleProject!)
        }
        catch (error) {
            console.log(error);
            this.setLoading(false);
        }
    }

    removeProperty = async (propId: string) => {
        this.loading = true;
        const id = this.selectedRuleProject!.id;
        try {
            await agent.RuleProjects.removeProperty(id, propId);
            runInAction(() => {
                this.selectedRuleProject!.properties = (this.selectedRuleProject?.properties ?? []).filter(a => a.id !== id);
                this.loading = false;
            })
        } catch (error) {
            console.log(error);
            this.setLoading(false);
        }
    }

    updateMembership = async (username: string) => {
        const user = await agent.Account.getUser(username);
        this.loading = true;
        try {
            await agent.RuleProjects.updateMember(this.selectedRuleProject!.id, user.username);
            runInAction(() => {
                if (this.selectedRuleProject?.members.some(x => x.username == user.username)) {
                    this.selectedRuleProject.members = this.selectedRuleProject.members?.filter(a => a.username !== user.username);
                } else {
                    const member = new Profile(user!);
                    this.selectedRuleProject?.members?.push(member);
                }
                this.ruleProjectRegistry.set(this.selectedRuleProject!.id, this.selectedRuleProject!)
            })
        } catch (error) {
            console.log(error);
        } finally {
            runInAction(() => this.loading = false);
        }
    }

    // updateRule = async (rule: RuleFormValues) => {
    //     try {
    //         const ruleToUpdate = {
    //             ...rule,
    //             conditions: rule.conditions.map(c => ({
    //                 ...c,
    //                 logicalOperator: c.logicalOperator,
    //                 subConditions: c.subConditions?.map(sub => ({
    //                     ...sub,
    //                     logicalOperator: sub.logicalOperator
    //                 }))
    //             })),
    //             actions: rule.actions.map(a => ({ ...a })),
    //         };
    //         await agent.Rules.update(ruleToUpdate);
    //         runInAction(() => {
    //             if (rule.id) {
    //                 let updatedRule = { ...this.getRule(rule.id), ...rule }
    //                 this.ruleRegistry.set(rule.id, updatedRule as Rule);
    //                 this.selectedRule = updatedRule as Rule;
    //             }
    //         })
    //     } catch (error) {
    //         console.log(error);
    //     }
    // }

    deleteRuleProject = async (id: string) => {
        this.loading = true;
        try {
            await agent.RuleProjects.delete(id);
            runInAction(() => {
                this.ruleProjectRegistry.delete(id);
                this.loading = false;
            })
        } catch (error) {
            console.log(error);
            this.setLoading(false);
        }
    }

    clearSelectedRuleProject = () => {
        this.selectedRuleProject = undefined;
    }
}