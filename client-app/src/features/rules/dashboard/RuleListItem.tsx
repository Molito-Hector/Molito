import { Link } from 'react-router-dom';
import { Item } from 'semantic-ui-react';
import { Rule } from '../../../app/models/rule';

interface Props {
    rule: Rule
}

export default function RuleListItem({ rule }: Props) {
    return (
        <Item>
            <Item.Content>
                <Item.Header as='a'>{rule.name}</Item.Header>
                <Item.Meta>
                    <Link to={`/rules/${rule.id}`}>
                        View Details
                    </Link>
                </Item.Meta>
                <Item.Description>
                    Conditions: {rule.conditions.length} <br />
                    Actions: {rule.actions.length}
                </Item.Description>
            </Item.Content>
        </Item>
    )
}