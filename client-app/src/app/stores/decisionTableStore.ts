import { makeAutoObservable, runInAction } from "mobx";
import agent from "../api/agent";
import { Pagination, PagingParams } from "../models/pagination";
import { DTFormValues, DecisionTable } from "../models/decisionTable";
import { store } from "./store";

export default class DecisionTableStore {
    tableRegistry = new Map<string, DecisionTable>();
    selectedTable: DecisionTable | undefined = undefined;
    editMode = false;
    loading = false;
    loadingInitial = false;
    pagination: Pagination | null = null;
    pagingParams = new PagingParams();
    predicate = new Map().set('all', true);

    constructor() {
        makeAutoObservable(this);
    }

    loadTable = async (id: string) => {
        let table = this.getTable(id);
        if (table && table.rows !== undefined) {
            this.selectedTable = table;
            return table;
        }
        else {
            this.setLoadingInitial(true);
            try {
                table = await agent.DecisionTables.details(id);
                table.createdAt = new Date(table.createdAt);
                this.setTable(table);
                runInAction(() => this.selectedTable = table);
                this.setLoadingInitial(false);
                return table;
            } catch (error) {
                console.log(error);
                this.setLoadingInitial(false);
            }
        }
    }

    private setTable = (table: DecisionTable) => {
        this.tableRegistry.set(table.id, table);
    }

    private getTable = (id: string) => {
        return this.tableRegistry.get(id);
    }

    setLoadingInitial = (state: boolean) => {
        this.loadingInitial = state;
    }

    setLoading = (state: boolean) => {
        this.loading = state;
    }

    createTable = async (table: DTFormValues) => {
        try {
            const tableToCreate = {
                ...table
            };
            await agent.DecisionTables.create(tableToCreate);
            const newTable = new DecisionTable(table);
            newTable.createdAt = new Date();
            this.setTable(newTable);
            store.ruleProjectStore.add(newTable);
            runInAction(() => {
                this.selectedTable = newTable;
            })
        } catch (error) {
            console.log(error);
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
    //             // actions: rule.actions.map(a => ({ ...a })),
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

    deleteTable = async (id: string) => {
        this.loading = true;
        try {
            await agent.DecisionTables.delete(id);
            runInAction(() => {
                this.tableRegistry.delete(id);
                store.ruleProjectStore.remove(id);
                this.loading = false;
            })
        } catch (error) {
            console.log(error);
            this.setLoading(false);
        }
    }

    clearSelectedTable = () => {
        this.selectedTable = undefined;
    }
}