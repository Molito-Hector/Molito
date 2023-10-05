import { Table } from "semantic-ui-react";
import { DecisionRow } from "../../app/models/decisionTable";
import { Action, Condition } from "../../app/models/rule";
import { DecisionTableRow } from "./DecisionTableRow";

interface Props {
    rows: DecisionRow[];
    conditions: Condition[];
    actions: Action[];
    editMode: boolean;
    handleCellValueChange: (row: DecisionRow, condition: Condition, newValue: string) => void;
    handleActionValueChange: (row: DecisionRow, action: Action, newValue: string) => void;
    handleDeleteRow: (rowId: string) => void;
}

export const DecisionTableBody: React.FC<Props> = ({ rows, conditions, actions, editMode, handleCellValueChange, handleActionValueChange, handleDeleteRow }) => {
    return (
        <Table.Body>
            {rows.length > 0 ? (
                rows.map((row, idx) => (
                    <DecisionTableRow
                        key={row.id}
                        row={row}
                        conditions={conditions}
                        actions={actions}
                        editMode={editMode}
                        handleCellValueChange={handleCellValueChange}
                        handleActionValueChange={handleActionValueChange}
                        handleDeleteRow={handleDeleteRow}
                    />
                ))
            ) : (
                <Table.Row>
                    <Table.Cell>This table currently has no data!</Table.Cell>
                </Table.Row>
            )}
        </Table.Body>
    );
};