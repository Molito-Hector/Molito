import { useEffect, useState } from "react";
import { Button, Checkbox, Form, Grid, Header, Icon, Input, Segment, Table } from "semantic-ui-react";
import { useStore } from "../../../app/stores/store";
import LoadingComponent from "../../../app/layout/LoadingComponent";
import { observer } from "mobx-react-lite";
import { useParams } from "react-router-dom";

interface TableRow {
    outputLabel: string;
    threshold: string;
}

interface FeatureRow {
    column: string;
}

export default observer(function RuleDetails() {

    const { ruleStore } = useStore();
    const { selectedRule: rule, loadRule, loadingInitial, clearSelectedRule } = ruleStore;
    const { id } = useParams();

    const [field, setField] = useState('');
    const [operator, setOperator] = useState('==');

    const [outputRows, setOutputRows] = useState<TableRow[]>([]);
    const [featureRows, setFeatureRows] = useState<FeatureRow[]>([]);

    function addRow() {
        const newRow = { outputLabel: '', threshold: '' };
        setOutputRows([...outputRows, newRow]);
    }

    function addFeatureRow() {
        const newRow = { column: '' };
        setFeatureRows([...featureRows, newRow]);
    }

    const RULETYPES = [
        { value: "anomalyDetection", label: "Anomaly Detection" },
    ];

    const ALGORITHMS = [
        { value: "isolationForest", label: "Isolation Forest" },
    ];

    const [editMode, setEditMode] = useState(false);

    useEffect(() => {
        if (id) loadRule(id);
        return () => clearSelectedRule();
    }, [id, loadRule, clearSelectedRule])

    if (loadingInitial || !rule) return <LoadingComponent />;

    return (
        <Segment clearing raised>
            <Segment clearing raised>
                <Header as='h3' dividing>
                    <Icon inverted color='purple' name='cube' />AI Rule Details - This feature is temporarily unavailable. We're working on updates and will restore it soon. Thank you for your patience!
                    <Button floated="right" color='teal' onClick={() => window.history.back()}>Back</Button>
                    <Button floated='right' color='purple' onClick={() => setEditMode(!editMode)}>
                        {editMode ? 'Cancel Edit' : 'Edit Mode'}
                    </Button>
                </Header>
                <p><strong>Name: </strong>{rule && rule.name}</p>
                <p><strong>Description: </strong>{rule && rule.description}</p>
            </Segment>
            <Segment clearing raised>
                <Grid>
                    <Grid.Column width={4}>
                        <Form>
                            <Form.Field disabled>
                                <label>Field</label>
                                <select value={field} onChange={(e) => {
                                    setField(e.target.value);
                                }}>
                                    {RULETYPES.map(prop => (
                                        <option key={prop.value} value={prop.value}>
                                            {prop.label}
                                        </option>
                                    ))}
                                </select>
                            </Form.Field>
                            <Form.Field disabled>
                                <label>Operator</label>
                                <select value={operator} onChange={(e) => setOperator(e.target.value)}>
                                    {ALGORITHMS.map(op => (
                                        <option key={op.value} value={op.value}>
                                            {op.label}
                                        </option>
                                    ))}
                                </select>
                            </Form.Field>
                        </Form>
                    </Grid.Column>
                    <Grid.Column width={4}>
                        <Table>
                            <Table.Header>
                                <Table.Row>
                                    <Table.HeaderCell>
                                        Columns to analyze:
                                    </Table.HeaderCell>
                                </Table.Row>
                            </Table.Header>
                            <Table.Body>
                                {featureRows.map((row, index) => (
                                    <Table.Row key={index}>
                                        <Table.Cell>
                                            <Input
                                                type="text"
                                                className="overrideStyle"
                                                value={row.column}
                                                onChange={(e) => {
                                                    const newRows = [...featureRows];
                                                    newRows[index].column = e.target.value;
                                                    setFeatureRows(newRows);
                                                }}
                                            />
                                        </Table.Cell>
                                    </Table.Row>
                                ))}
                            </Table.Body>
                        </Table>
                        {editMode && (
                            <Button onClick={addFeatureRow} style={{ margin: '0 0 5% 0' }} color='teal'>Add Column</Button>
                        )}
                    </Grid.Column>
                    <Grid.Column width={4}>
                        <Table>
                            <Table.Header>
                                <Table.Row>
                                    <Table.HeaderCell>Output Label</Table.HeaderCell>
                                    <Table.HeaderCell>Threshold</Table.HeaderCell>
                                </Table.Row>
                            </Table.Header>
                            <Table.Body>
                                {outputRows.map((row, index) => (
                                    <Table.Row key={index}>
                                        <Table.Cell>
                                            <Input
                                                type="text"
                                                className="overrideStyle"
                                                value={row.outputLabel}
                                                onChange={(e) => {
                                                    const newRows = [...outputRows];
                                                    newRows[index].threshold = e.target.value;
                                                    setOutputRows(newRows);
                                                }}
                                            />
                                        </Table.Cell>
                                        <Table.Cell>
                                            <Input
                                                type="text"
                                                className="overrideStyle"
                                                value={row.threshold}
                                                onChange={(e) => {
                                                    const newRows = [...outputRows];
                                                    newRows[index].threshold = e.target.value;
                                                    setOutputRows(newRows);
                                                }}
                                            />
                                        </Table.Cell>
                                    </Table.Row>
                                ))}
                            </Table.Body>
                        </Table>
                        {editMode && (
                            <Button onClick={addRow} style={{ margin: '0 0 5% 0' }} color='teal'>Add Value</Button>
                        )}
                    </Grid.Column>
                    <Grid.Column width={4}>
                        <p><strong>Upload your training data:</strong></p>
                        <Button disabled style={{ margin: '0 0 5% 0' }} color='teal'>Upload Training Data <Icon name='arrow up' /></Button>
                        <p><strong>Adjust your training parameters:</strong></p>
                        <Checkbox disabled={editMode ? false : true} label='Apply Data Transformations' />
                        <br />
                        <Checkbox disabled={editMode ? false : true} label='Limit Decision Tree size' />
                        <br />
                        <div style={{ margin: '0 0 5% 0' }} />
                        <Button disabled style={{ margin: '0 0 10% 0' }} color='teal'>Save & Train Model!</Button>
                    </Grid.Column>
                </Grid>
            </Segment>
        </Segment >
    )
})