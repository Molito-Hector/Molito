import { observer } from 'mobx-react-lite';
import { Card } from 'semantic-ui-react'
import { Condition, Rule } from '../../../app/models/rule';

interface Props {
    rule: Rule
}

function renderCondition(condition: Condition) {
    return (
        <li key={condition.id}>
            Field: {condition.field} <br />
            Operator: {condition.operator} <br />
            Value: {condition.value}
            {condition.subConditions && condition.subConditions.length > 0 && (
                <>
                    <div>Sub-Conditions (Logical Operator: {condition.logicalOperator}):</div>
                    <ul>
                        {condition.subConditions.map(subCondition => renderCondition(subCondition))}
                    </ul>
                </>
            )}
        </li>
    );
}

export default observer(function RuleDetailedInfo({ rule }: Props) {
    return (
        <Card fluid>
            <Card.Content>
                <Card.Header>{rule.name}</Card.Header>
                <Card.Description>
                    <div>Conditions:</div>
                    <ul>
                        {rule.conditions.map(condition => renderCondition(condition))}
                    </ul>
                    <div>Actions:</div>
                    {/* <ul>
                        {rule.actions.map(action => (
                            <li key={action.id}>
                                Name: {action.name}
                            </li>
                        ))}
                    </ul> */}
                </Card.Description>
            </Card.Content>
        </Card>
    )
})
