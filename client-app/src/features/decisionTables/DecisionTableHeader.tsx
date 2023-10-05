import { Table } from "semantic-ui-react";
import { Condition } from "../../app/models/rule";

interface Props {
    conditions: Condition[];
}

export const DecisionTableHeader: React.FC<Props> = ({ conditions }) => {
    return (
        <Table.Header>
            <Table.Row>
                <Table.HeaderCell>#</Table.HeaderCell>
                {conditions.map((condition) => (
                    <Table.HeaderCell key={condition.id}>{condition.field}</Table.HeaderCell>
                ))}
                <Table.HeaderCell>Action</Table.HeaderCell>
            </Table.Row>
        </Table.Header>
    );
};