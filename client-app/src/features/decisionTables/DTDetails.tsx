import { useEffect } from "react";
import { Grid, Segment, Table } from "semantic-ui-react";
import { useStore } from "../../app/stores/store";
import LoadingComponent from "../../app/layout/LoadingComponent";
import { observer } from "mobx-react-lite";
import { useParams } from "react-router-dom";
import { Condition } from "../../app/models/rule";

export default observer(function DTDetails() {

    const { decisionTableStore } = useStore();
    const { selectedTable: table, loadTable, loadingInitial, clearSelectedTable } = decisionTableStore;
    const { id } = useParams();

    useEffect(() => {
        if (id) loadTable(id);
        return () => clearSelectedTable();
    }, [id, loadTable, clearSelectedTable])

    if (loadingInitial || !table) return <LoadingComponent />;

    const conditions = table.conditions as Condition[];
    const rows = table.rows.map((row, index) => ({ ...row, index }));

    return (
        <Segment clearing raised>
            <p>{table.name}</p>
            <p>{table.description}</p>
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
                                                {row.values.find((value) => value.conditionId === condition.id)?.value}
                                            </Table.Cell>
                                        ))}
                                        <Table.Cell>{row.actions[0].modificationType + " " + row.actions[0].targetProperty + " to: " + row.actions[0].modificationValue}</Table.Cell>
                                    </Table.Row>
                                ))
                            ) : (
                                <p>Nada!</p>
                            )}
                        </Table.Body>
                    </Table>
                </Grid.Column>
            </Grid>
        </Segment >
    )
})