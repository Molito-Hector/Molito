import { makeAutoObservable, runInAction } from "mobx";
import agent from "../api/agent";
import { Pagination, PagingParams } from "../models/pagination";
import { DTFormValues, DecisionTable } from "../models/decisionTable";
import { store } from "./store";
import { Condition } from "../models/rule";

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

    populateTable = async (table: DecisionTable) => {
        this.loading = true;
        try {
            await agent.DecisionTables.populate(table.id, table);
            runInAction(() => {
                if (table.id) {
                    let updatedTable = { ...this.getTable(table.id), ...table }
                    this.tableRegistry.set(table.id, updatedTable as DecisionTable);
                    this.selectedTable = updatedTable as DecisionTable;
                    this.loading = false;
                }
            })
        } catch (error) {
            console.log(error);
        }
    }

    addTableColumn = async (condition: Condition) => {
        console.log('Starting addTableColumn', condition);

        this.loading = true;
        var id = this.selectedTable!.id;
        try {
            console.log('Before API Call: Current Conditions', this.selectedTable?.conditions);

            await agent.DecisionTables.addColumn(id, condition, 'Table');

            console.log('After API Call: Current Conditions', this.selectedTable?.conditions);

            runInAction(() => {
                this.selectedTable?.conditions.push(condition);
                this.loading = false;
            });

            console.log('After runInAction: Current Conditions', this.selectedTable?.conditions);
        } catch (error) {
            console.log(error);
        }
    }

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