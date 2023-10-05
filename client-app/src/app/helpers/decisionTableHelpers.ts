import { DecisionRow, DecisionTable } from "../models/decisionTable";
import { Condition } from "../models/rule";
import { v4 as uuid } from 'uuid';
import { RuleProperty } from "../models/ruleProject";

export const addRowToTable = (table: DecisionTable): DecisionTable => {
    const newRow: DecisionRow = {
        id: uuid(),
        tableId: table.id,
        values: [],
        actionValues: []
    };

    table.conditions.forEach(condition => {
        newRow.values.push({
            conditionId: condition.id!,
            value: '',
            decisionRowId: newRow.id
        });
    });

    table.actions.forEach(action => {
        newRow.actionValues.push({
            actionId: action.id!,
            value: '',
            decisionRowId: newRow.id
        });
    });
    return { ...table, rows: [...table.rows, newRow] };
};

export const addColumnToTable = (table: DecisionTable, condition: Condition): DecisionTable => {
    const updatedTable = { ...table };

    if (!updatedTable.conditions) {
        updatedTable.conditions = [];
    }
    updatedTable.conditions.push(condition);

    if (updatedTable.rows) {
        for (const row of updatedTable.rows) {
            if (!row.values) {
                row.values = [];
            }
            row.values.push({ conditionId: condition.id!, value: '' });
        }
    }

    return updatedTable;
};

export const filterProperties = (properties: RuleProperty[], filterDirection: string): RuleProperty[] => {
    let filteredProps: RuleProperty[] = [];
    let directionsToFilter: string[] = [];

    if (filterDirection === 'Input') {
        directionsToFilter = ['I', 'B'];
    } else if (filterDirection === 'Output') {
        directionsToFilter = ['O', 'B'];
    }

    properties.forEach(prop => {
        if (prop.subProperties.length === 0) {
            if (directionsToFilter.includes(prop.direction)) {
                filteredProps.push(prop);
            }
        } else {
            const subProps = prop.subProperties.filter(subProp =>
                directionsToFilter.includes(subProp.direction)
            );
            filteredProps = [...filteredProps, ...subProps];
        }
    });

    return filteredProps;
}