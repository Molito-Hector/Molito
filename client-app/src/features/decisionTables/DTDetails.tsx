import { useEffect, useState } from "react";
import { Button, Header, Icon, Segment, Table } from "semantic-ui-react";
import { useStore } from "../../app/stores/store";
import LoadingComponent from "../../app/layout/LoadingComponent";
import { observer } from "mobx-react-lite";
import { useParams } from "react-router-dom";
import { Action, Condition } from "../../app/models/rule";
import { DecisionRow, DecisionTable } from "../../app/models/decisionTable";
import AddConditionModal from "./AddConditionModal";
import { v4 as uuid } from 'uuid';
import { DecisionTableBody } from "./DecisionTableBody";
import { addRowToTable } from "../../app/helpers/decisionTableHelpers";
import ActionModal from "./ActionModal";
import DeleteConditionModal from "./DeleteConditionModal";
import '../../app/layout/styles.css';

export default observer(function DTDetails() {

    const { decisionTableStore } = useStore();
    const { selectedTable: table, loadTable, loadingInitial, clearSelectedTable, populateTable, loading, addTableColumn, addTableActionColumn } = decisionTableStore;
    const { id } = useParams();

    const { ruleProjectStore } = useStore();
    const { selectedRuleProject: ruleProject, loadRuleProject } = ruleProjectStore;

    const [editMode, setEditMode] = useState(false);
    const [modifiedTable, setModifiedTable] = useState<DecisionTable | undefined>(undefined);
    const [isAddConditionModalOpen, setIsAddConditionModalOpen] = useState(false);
    const [isActionModalOpen, setIsActionModalOpen] = useState(false);

    const [conditionToDelete, setConditionToDelete] = useState<string | null>(null);
    const [deleteType, setDeleteType] = useState<string | null>(null);
    const [deleteConditionModalOpen, setDeleteModalOpen] = useState(false);

    enum SortDirection {
        ASCENDING = 'ascending',
        DESCENDING = 'descending',
    }

    const [sortState, setSortState] = useState<{
        column: string;
        direction: SortDirection;
    }>({
        column: '',
        direction: SortDirection.ASCENDING
    });

    const [sortedRows, setSortedRows] = useState<DecisionRow[]>([]);

    useEffect(() => {
        if (modifiedTable) {
            setSortedRows(modifiedTable.rows.map((row) => ({ ...row })));
        }
    }, [modifiedTable]);

    const handleOpenDeleteModal = (conditionID: string, type: string) => {
        setConditionToDelete(conditionID);
        setDeleteType(type);
        setDeleteModalOpen(true);
    };

    useEffect(() => {
        if (id) loadTable(id);
        return () => clearSelectedTable();
    }, [id, loadTable, clearSelectedTable])

    useEffect(() => {
        setModifiedTable(table);
    }, [table]);

    useEffect(() => {
        if (id) loadTable(id);
        if (table?.ruleProjectId && !ruleProject) {
            loadRuleProject(table.ruleProjectId);
        }
        return () => {
            clearSelectedTable();
        }
    }, [id, loadTable, clearSelectedTable, table, ruleProject, loadRuleProject]);

    if (loadingInitial || !table) return <LoadingComponent />;
    if (loading) return <LoadingComponent content="Saving changes..." />;

    const conditions = (modifiedTable && modifiedTable.conditions) || [];
    const actions = (modifiedTable && modifiedTable.actions) || [];

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

    const handleActionValueChange = (row: DecisionRow, action: Action, newValue: string) => {
        if (!modifiedTable) return;

        const newTable = { ...modifiedTable };
        const targetRow = newTable.rows?.find(r => r.id === row.id);

        if (targetRow) {
            const targetValue = targetRow.actionValues.find(actionValue => actionValue.actionId === action.id);

            if (targetValue) {
                targetValue.value = newValue;
                setModifiedTable(newTable);
            } else {
                console.warn("Target value for action not found");
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
        const updatedTable = addRowToTable(modifiedTable);
        setModifiedTable(updatedTable);
    };

    const removeRow = (rowId: string) => {
        try {
            setModifiedTable(prevState => {
                if (!prevState) return;
                const updatedRows = prevState.rows.filter(row => row.id !== rowId);
                return {
                    ...prevState,
                    rows: updatedRows
                };
            });
        } catch (error) {
            console.error("Failed to save changes: ", error);
        }
    }

    const addColumn = async (field: string, operator: string) => {
        if (!modifiedTable) return;

        const newCondition: Condition = {
            id: uuid(),
            field: field,
            operator: operator,
            value: '',
            actions: [],
            tableColumnIndex: modifiedTable.conditions.length
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

    const addActionColumn = async (targetProp: string, modType: string) => {
        if (!modifiedTable) return;

        const newAction: Action = {
            id: uuid(),
            name: modType + ' ' + targetProp,
            modificationType: modType,
            targetProperty: targetProp,
            modificationValue: ''
        };

        try {
            await addTableActionColumn(newAction);
            setModifiedTable(prevState => {
                if (!prevState) return;

                const updatedRows = prevState.rows.map(row => {
                    const newRow = { ...row };
                    newRow.actionValues.push({
                        actionId: newAction.id!,
                        value: ''
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

    const handleAddAction = (modType: string, targetProp: string) => {
        addActionColumn(modType, targetProp);
    }

    const handleDeleteColumn = async (columnId: string, type: string) => {
        try {
            if (type === 'condition') {
                await decisionTableStore.deleteColumn(columnId);
                setModifiedTable(prevState => {
                    if (!prevState) return;

                    const updatedConditions = prevState.conditions.filter(c => c.id !== columnId);
                    updatedConditions.forEach((cond, idx) => {
                        cond.tableColumnIndex = idx;
                    });
                    setEditMode(false);
                    setDeleteModalOpen(false);

                    return {
                        ...prevState,
                        conditions: updatedConditions
                    };
                })
            } else if (type === 'action') {
                await decisionTableStore.deleteActionColumn(columnId);
                setModifiedTable(prevState => {
                    if (!prevState) return;

                    const updatedActions = prevState.actions.filter(c => c.id !== columnId);
                    setEditMode(false);
                    setDeleteModalOpen(false);

                    return {
                        ...prevState,
                        actions: updatedActions
                    };
                })
            }

        } catch (error) {
            console.error("Failed to delete column: ", error);
        }
    };

    const handleSort = (type: string, columnId: string) => {
        console.log('sorting');
        console.log(type + ' ' + columnId);

        if (sortState.column === columnId) {
            setSortState(prev => ({
                column: prev.column,
                direction: prev.direction === SortDirection.ASCENDING ? SortDirection.DESCENDING : SortDirection.ASCENDING
            }));
        } else {
            setSortState({
                column: columnId,
                direction: SortDirection.ASCENDING
            });
        }

        setSortedRows(prevRows => {
            let newRows = [...prevRows];
            if (type === 'condition') {
                newRows.sort((a, b) => {
                    const aValue = a.values.find(val => val.conditionId === columnId)?.value || '';
                    const bValue = b.values.find(val => val.conditionId === columnId)?.value || '';

                    return sortState.direction === 'ascending'
                        ? aValue.localeCompare(bValue)
                        : bValue.localeCompare(aValue);
                });
            } else if (type === 'action') {
                newRows.sort((a, b) => {
                    const aValue = a.actionValues.find(val => val.actionId === columnId)?.value || '';
                    const bValue = b.actionValues.find(val => val.actionId === columnId)?.value || '';

                    return sortState.direction === 'ascending'
                        ? aValue.localeCompare(bValue)
                        : bValue.localeCompare(aValue);
                });
            }

            return newRows;
        });
    };

    return (
        <Segment clearing raised>
            <Segment clearing raised>
                <Header as='h3' dividing>
                    <Icon inverted color='purple' name='table' />Decision Table Details
                    <Button floated="right" color='teal' onClick={() => window.history.back()}>Back</Button>
                    <Button floated='right' color='purple' onClick={() => setEditMode(!editMode)}>
                        {editMode ? 'Cancel Edit' : 'Edit Mode'}
                    </Button>
                </Header>
                <p><strong>Name: </strong>{modifiedTable && modifiedTable.name}</p>
                <p><strong>Description: </strong>{modifiedTable && modifiedTable.description}</p>
            </Segment>
            {editMode && (
                <Segment clearing raised>
                    <Button basic color='purple' onClick={addRow}>Add Row</Button>
                    <Button basic color='teal' onClick={() => setIsAddConditionModalOpen(true)}>Add Condition</Button>
                    <Button basic color='purple' onClick={() => setIsActionModalOpen(true)}>Add Action</Button>

                    <Button positive onClick={handleSaveChanges}>Save Changes</Button>
                    <AddConditionModal
                        open={isAddConditionModalOpen}
                        onClose={() => setIsAddConditionModalOpen(false)}
                        onSubmit={handleAddCondition}
                        ruleProperties={ruleProject?.properties || []}
                    />
                    <ActionModal
                        open={isActionModalOpen}
                        onClose={() => setIsActionModalOpen(false)}
                        onSubmit={handleAddAction}
                        ruleProperties={ruleProject?.properties || []}
                    />
                    <DeleteConditionModal
                        open={deleteConditionModalOpen}
                        type={deleteType!}
                        onClose={() => setDeleteModalOpen(false)}
                        onSubmit={handleDeleteColumn}
                        condId={conditionToDelete!}
                        loading={loading}
                    />
                </Segment>
            )}
            <Segment clearing raised>
                <div style={{ maxWidth: '100%', overflowX: 'auto' }}>
                    <Table sortable celled striped>
                        <Table.Header>
                            <Table.Row>
                                {conditions.slice().sort((a, b) => a.tableColumnIndex! - b.tableColumnIndex!).map((condition) => (
                                    <Table.HeaderCell className="hol" key={condition.id}
                                        sorted={sortState.column === condition.id ? sortState.direction : undefined}
                                        onClick={() => handleSort('condition', condition.id!)}
                                    >
                                        Condition: {condition.field} {condition.operator}
                                        {editMode && (
                                            <Icon
                                                name='trash'
                                                style={{ float: 'right', opacity: 0.3, transition: 'opacity 0.2s', cursor: 'pointer' }}
                                                className="deletable-column"
                                                onClick={() => handleOpenDeleteModal(condition.id!, 'condition')}
                                            />
                                        )}
                                    </Table.HeaderCell>
                                ))}
                                {actions.map((action) => (
                                    <Table.HeaderCell key={action.id}
                                        sorted={sortState.column === action.id ? sortState.direction : undefined}
                                        onClick={() => handleSort('action', action.id!)}
                                    >
                                        Action: {action.name}
                                        {editMode && (
                                            <Icon
                                                name='trash'
                                                style={{ float: 'right', opacity: 0.3, transition: 'opacity 0.2s', cursor: 'pointer' }}
                                                className="deletable-column"
                                                onClick={() => handleOpenDeleteModal(action.id!, 'action')}
                                            />
                                        )}
                                    </Table.HeaderCell>
                                ))}
                            </Table.Row>
                        </Table.Header>
                        <DecisionTableBody
                            rows={sortedRows}
                            conditions={conditions}
                            actions={actions}
                            editMode={editMode}
                            handleCellValueChange={handleCellValueChange}
                            handleActionValueChange={handleActionValueChange}
                            handleDeleteRow={removeRow}
                        />
                    </Table>
                </div>
            </Segment>
        </Segment>
    )
})