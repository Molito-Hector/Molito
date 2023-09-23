import { useEffect, useState } from "react";
import { Button, Grid, Header, Icon, Input, Segment, Table } from "semantic-ui-react";
import { useStore } from "../../app/stores/store";
import LoadingComponent from "../../app/layout/LoadingComponent";
import { observer } from "mobx-react-lite";
import { useParams } from "react-router-dom";
import { Condition } from "../../app/models/rule";
import { DecisionRow, DecisionTable } from "../../app/models/decisionTable";
import AddConditionModal from "./AddConditionModal";
import { v4 as uuid } from 'uuid';

export default observer(function DTDetailsNew() {

    const { decisionTableStore } = useStore();
    const { selectedTable: table, loadTable, loadingInitial, clearSelectedTable, populateTable, loading, addTableColumn } = decisionTableStore;
    const { id } = useParams();

    const [editMode, setEditMode] = useState(false);
    const [modifiedTable, setModifiedTable] = useState<DecisionTable | undefined>(undefined);
    const [isAddConditionModalOpen, setIsAddConditionModalOpen] = useState(false);

    useEffect(() => {
        if (id) loadTable(id);
        return () => clearSelectedTable();
    }, [id, loadTable, clearSelectedTable])

    useEffect(() => {
        setModifiedTable(table);
    }, [table]);

    if (loadingInitial || !table) return <LoadingComponent />;
    if (loading) return <LoadingComponent content="Saving changes..." />;

    const conditions = (modifiedTable && modifiedTable.conditions) || [];
    const rows = (modifiedTable && modifiedTable.rows.map((row, index) => ({ ...row, index }))) || [];

    const handleCellValueChange = (row: DecisionRow, condition: Condition, newValue: string) => {
        if (!modifiedTable) return;

        const newTable = { ...modifiedTable };
        const targetRow = newTable.rows?.find(r => r.id === row.id);

        if (targetRow) {
            const targetValue = targetRow.values.find(value => value.conditionId === condition.id);

            if (targetValue) {
                targetValue.value = newValue;
                setModifiedTable(newTable);
            } else {
                console.warn("Target value for condition not found");
            }
        } else {
            console.warn("Target row not found");
        }
    };

    const handleSaveChanges = async () => {
        try {
            await populateTable(modifiedTable!);
            setEditMode(false);
        } catch (error) {
            console.error("Failed to save changes: ", error);
        }
    }

    const addRow = () => {
        if (!modifiedTable) return;

        const newRow: DecisionRow = {
            id: uuid(),
            tableId: modifiedTable.id,
            values: [],
            actions: []
        };

        modifiedTable.conditions.forEach(condition => {
            newRow.values.push({
                conditionId: condition.id!,
                value: '',
                decisionRowId: newRow.id
            });
            newRow.actions.push({
                name: condition.field + '_condition',
                targetProperty: '',
                modificationValue: '',
                modificationType: '',
                rowId: newRow.id
            })
        })

        const newTable = { ...modifiedTable } as DecisionTable;
        newTable.rows.push(newRow);
        setModifiedTable(newTable);
    };

    const addColumn = async (field: string, operator: string) => {
        if (!modifiedTable) return;

        const newCondition: Condition = {
            id: uuid(),
            field: field,
            operator: operator,
            value: '',
            actions: []
        };

        try {
            await addTableColumn(newCondition);
            setModifiedTable(prevState => {
                if (!prevState) return;

                const updatedRows = prevState.rows.map(row => {
                    const newRow = { ...row };
                    newRow.values.push({
                        conditionId: newCondition.id!,
                        value: ''
                    });
                    newRow.actions.push({
                        name: newCondition.field + '_condition',
                        targetProperty: '',
                        modificationValue: '',
                        modificationType: ''
                    });
                    return newRow;
                });

                setEditMode(false);

                return {
                    ...prevState,
                    rows: updatedRows
                };
            });
        } catch (error) {
            console.error("Failed to save changes: ", error);
        }
    };

    const handleAddCondition = (field: string, operator: string) => {
        addColumn(field, operator);
    };

    return (
        <Segment clearing raised>
            <Segment clearing raised>
                <Header as='h3' dividing>
                    <Icon inverted color='purple' name='table' />Decision Table Details
                    <Button floated="right" color='teal' onClick={() => window.history.back()}>Back</Button>
                    <Button onClick={() => setEditMode(!editMode)}>
                        {editMode ? 'Cancel Edit' : 'Edit Mode'}
                    </Button>
                    {editMode && (
                        <>
                            <Button onClick={addRow}>Add Row</Button>
                            <Button onClick={handleSaveChanges}>Save Changes</Button>
                            <Button onClick={() => setIsAddConditionModalOpen(true)}>Add Condition</Button>
                            <AddConditionModal
                                open={isAddConditionModalOpen}
                                onClose={() => setIsAddConditionModalOpen(false)}
                                onSubmit={handleAddCondition}
                            />

                        </>
                    )}
                </Header>
                <p><strong>Name: </strong>{modifiedTable && modifiedTable.name}</p>
                <p><strong>Description: </strong>{modifiedTable && modifiedTable.description}</p>
            </Segment>
            <Grid>
                <Grid.Column width={16}>
                    <Table celled>
                        <Table.Header>
                            <Table.Row>
                                <Table.HeaderCell>#</Table.HeaderCell>
                                {conditions.map((condition) => (
                                    <Table.HeaderCell key={condition.id}>{condition.field}</Table.HeaderCell>
                                ))}
                                <Table.HeaderCell>Action</Table.HeaderCell>
                            </Table.Row>
                        </Table.Header>
                        <Table.Body>
                            {rows.length > 0 && rows[0].actions.length > 0 ? (
                                rows.map((row) => (
                                    <Table.Row key={row.id}>
                                        <Table.Cell>{row.index}</Table.Cell>
                                        {conditions.map((condition) => (
                                            <Table.Cell key={condition.id}>
                                                {editMode ? (
                                                    <Input
                                                        type="text"
                                                        value={row.values.find((value) => value.conditionId === condition.id)?.value}
                                                        onChange={(e) => handleCellValueChange(row, condition, e.target.value)}
                                                    />
                                                ) : (
                                                    row.values.find((value) => value.conditionId === condition.id)?.value
                                                )}
                                            </Table.Cell>
                                        ))}
                                        <Table.Cell>{row.actions[0].modificationType + " " + row.actions[0].targetProperty + " to: " + row.actions[0].modificationValue}</Table.Cell>
                                    </Table.Row>
                                ))
                            ) : (
                                <Table.Row>
                                    <Table.Cell>This table currently has no data!</Table.Cell>
                                </Table.Row>
                            )}
                        </Table.Body>
                    </Table>
                </Grid.Column>
            </Grid>
        </Segment >
    )
})