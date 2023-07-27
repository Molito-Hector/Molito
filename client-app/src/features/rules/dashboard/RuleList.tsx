import { useStore } from '../../../app/stores/store';
import { observer } from 'mobx-react-lite';
import RuleListItem from './RuleListItem';
import { Header, Item, Segment, Table } from 'semantic-ui-react';
import { Link } from 'react-router-dom';

export default observer(function ActivityList() {
    const { ruleStore } = useStore();
    const { rulesByName } = ruleStore;

    return (
        <Segment clearing>
            <Header content='Decision Library' sub color='teal' />
            <Table celled selectable>
                <Table.Header>
                    <Table.Row>
                        <Table.HeaderCell>Rule Name</Table.HeaderCell>
                        <Table.HeaderCell>Conditions</Table.HeaderCell>
                        <Table.HeaderCell>Actions</Table.HeaderCell>
                        <Table.HeaderCell></Table.HeaderCell>
                    </Table.Row>
                </Table.Header>
                <Table.Body>
                    {rulesByName.map((rule) => (
                        <Table.Row>
                            <Table.Cell>{rule.name}</Table.Cell>
                            <Table.Cell>{rule.conditions.length}</Table.Cell>
                            <Table.Cell>{rule.actions.length}</Table.Cell>
                            <Table.Cell>
                                <Link to={`/rules/${rule.id}`}>
                                    View Details
                                </Link>
                            </Table.Cell>
                        </Table.Row>
                    ))}
                </Table.Body>
            </Table>
            {/* <Item.Group divided>
                {rulesByName.map((rule) => (
                    <RuleListItem key={rule.id} rule={rule} />
                ))}
            </Item.Group> */}
        </Segment>
    )
})