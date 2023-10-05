import { makeAutoObservable, runInAction } from "mobx";
import agent from "../api/agent";
import { Pagination, PagingParams } from "../models/pagination";
import { DTFormValues, DecisionTable } from "../models/decisionTable";
import { store } from "./store";
import { Action, Condition } from "../models/rule";

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
            runInAction(() => {
                this.loading = false;
            });
        }
    }

    addTableColumn = async (condition: Condition) => {
        this.loading = true;
        var id = this.selectedTable!.id;
        try {
            await agent.DecisionTables.addColumn(id, condition, 'Table');
            runInAction(() => {
                this.selectedTable?.conditions.push(condition);
                this.loading = false;
            });
        } catch (error) {
            console.log(error);
            runInAction(() => {
                this.loading = false;
            });
        }
    }

    addTableActionColumn = async (action: Action) => {
        this.loading = true;
        var id = this.selectedTable!.id;
        try {
            await agent.DecisionTables.addActionColumn(id, action, 'Table');
            runInAction(() => {
                this.selectedTable?.actions.push(action);
                this.loading = false;
            });
        } catch (error) {
            console.log(error);
            runInAction(() => {
                this.loading = false;
            });
        }
    }

    // editActionColumn = async (action: Action) => {
    //     this.loading = true;
    //     var id = this.selectedTable!.id;
    //     try {
    //         await agent.DecisionTables.editActionColumn(id, action);
    //         runInAction(() => {
    //             if (this.selectedTable) {
    //                 this.selectedTable.rows.forEach(row => {
    //                     if (row.actions.length > 0) {
    //                         row.actions[0].modificationType = action.modificationType;
    //                         row.actions[0].targetProperty = action.targetProperty;
    //                     }
    //                 })
    //             }
    //             this.loading = false;
    //         });
    //     } catch (error) {
    //         console.log(error);
    //         runInAction(() => {
    //             this.loading = false;
    //         });
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

    deleteColumn = async (id: string) => {
        this.loading = true;
        const tableId = this.selectedTable!.id;
        try {
            await agent.DecisionTables.deleteColumn(id);
            runInAction(() => {
                this.loadTable(tableId);
                this.loading = false;
            })
        } catch (error) {
            console.log(error);
            this.setLoading(false);
        }
    }

    deleteActionColumn = async (id: string) => {
        this.loading = true;
        const tableId = this.selectedTable!.id;
        try {
            await agent.DecisionTables.deleteActionColumn(id);
            runInAction(() => {
                this.loadTable(tableId);
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