import { makeAutoObservable, reaction, runInAction } from "mobx";
import agent from "../api/agent";
import { Pagination, PagingParams } from "../models/pagination";
import { Rule, RuleFormValues } from "../models/rule";

export default class RuleStore {
    ruleRegistry = new Map<string, Rule>();
    selectedRule: Rule | undefined = undefined;
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
                this.ruleRegistry.clear();
                this.loadRules();
            }
        )
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

    get rulesByName() {
        return Array.from(this.ruleRegistry.values()).sort((a, b) => {
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

    get groupedRules() {
        return Object.entries(
            this.rulesByName.reduce((rules, rule) => {
                const name = rule.name;
                rules[name] = rules[name] ? [...rules[name], rule] : [rule];
                return rules;
            }, {} as { [key: string]: Rule[] })
        )
    }

    loadRules = async () => {
        this.setLoadingInitial(true);
        try {
            const result = await agent.Rules.list(this.axiosParams);
            result.data.forEach(rule => {
                this.setRule(rule);
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

    loadRule = async (id: string) => {
        let rule = this.getRule(id);
        if (rule) {
            this.selectedRule = rule;
            return rule;
        }
        else {
            this.setLoadingInitial(true);
            try {
                rule = await agent.Rules.details(id);
                this.setRule(rule);
                runInAction(() => this.selectedRule = rule);
                this.setLoadingInitial(false);
                return rule;
            } catch (error) {
                console.log(error);
                this.setLoadingInitial(false);
            }
        }
    }

    private setRule = (rule: Rule) => {
        this.ruleRegistry.set(rule.id, rule);
    }

    private getRule = (id: string) => {
        return this.ruleRegistry.get(id);
    }

    setLoadingInitial = (state: boolean) => {
        this.loadingInitial = state;
    }

    setLoading = (state: boolean) => {
        this.loading = state;
    }

    createRule = async (rule: RuleFormValues) => {
        try {
            const ruleToCreate = {
                ...rule,
                conditions: rule.conditions.map(c => ({
                    ...c,
                    logicalOperator: c.logicalOperator,
                    subConditions: c.subConditions?.map(sub => ({
                        ...sub,
                        logicalOperator: sub.logicalOperator
                    }))
                })),
                actions: rule.actions.map(a => ({ ...a })),
            };
            await agent.Rules.create(ruleToCreate);
            const newRule = new Rule(rule);
            this.setRule(newRule);
            runInAction(() => {
                this.selectedRule = newRule;
            })
        } catch (error) {
            console.log(error);
        }
    }

    updateRule = async (rule: RuleFormValues) => {
        try {
            const ruleToUpdate = {
                ...rule,
                conditions: rule.conditions.map(c => ({
                    ...c,
                    logicalOperator: c.logicalOperator,
                    subConditions: c.subConditions?.map(sub => ({
                        ...sub,
                        logicalOperator: sub.logicalOperator
                    }))
                })),
                actions: rule.actions.map(a => ({ ...a })),
            };
            await agent.Rules.update(ruleToUpdate);
            runInAction(() => {
                if (rule.id) {
                    let updatedRule = { ...this.getRule(rule.id), ...rule }
                    this.ruleRegistry.set(rule.id, updatedRule as Rule);
                    this.selectedRule = updatedRule as Rule;
                }
            })
        } catch (error) {
            console.log(error);
        }
    }

    deleteRule = async (id: string) => {
        this.loading = true;
        try {
            await agent.Rules.delete(id);
            runInAction(() => {
                this.ruleRegistry.delete(id);
                this.loading = false;
            })
        } catch (error) {
            console.log(error);
            this.setLoading(false);
        }
    }
    clearSelectedRule = () => {
        this.selectedRule = undefined;
    }
}