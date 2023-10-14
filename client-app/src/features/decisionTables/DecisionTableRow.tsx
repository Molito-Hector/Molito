import { Icon, Input, Table } from "semantic-ui-react";
import { DecisionRow } from "../../app/models/decisionTable";
import { Action, Condition } from "../../app/models/rule";
import '../../app/layout/styles.css';

interface Props {
    row: DecisionRow;
    conditions: Condition[];
    actions: Action[];
    editMode: boolean;
    handleCellValueChange: (row: DecisionRow, condition: Condition, newValue: string) => void;
    handleActionValueChange: (row: DecisionRow, action: Action, newValue: string) => void;
    handleDeleteRow: (rowId: string) => void;
}

export const DecisionTableRow: React.FC<Props> = ({ row, conditions, actions, editMode, handleCellValueChange, handleActionValueChange, handleDeleteRow }) => {
    return (
        <Table.Row key={row.id}>
            {conditions.map((condition) => (
                <Table.Cell key={condition.id}>
                    {editMode ? (
                        <Input
                            type="text"
                            className="overrideStyle"
                            value={row.values.find((value) => value.conditionId === condition.id)?.value}
                            onChange={(e) => handleCellValueChange(row, condition, e.target.value)}
                        />
                    ) : (
                        row.values.find((value) => value.conditionId === condition.id)?.value
                    )}
                </Table.Cell>
            ))}
            {actions.map((action) => (
                <Table.Cell key={action.id}>
                    {editMode ? (
                        <Input
                            type="text"
                            className="overrideStyle"
                            value={row.actionValues.find((value) => value.actionId === action.id)?.value}
                            onChange={(e) => handleActionValueChange(row, action, e.target.value)}
                        />
                    ) : (
                        row.actionValues.find((value) => value.actionId === action.id)?.value
                    )}
                </Table.Cell>
            ))}
            {editMode && (
                <Table.Cell collapsing>
                    <Icon
                        name='trash'
                        style={{ float: 'right', opacity: 0.3, transition: 'opacity 0.2s', cursor: 'pointer' }}
                        className="deletable-column"
                        onClick={() => handleDeleteRow(row.id!)}
                    />
                </Table.Cell>
            )}
        </Table.Row>
    );
};