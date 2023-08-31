import { observer } from 'mobx-react-lite';
import { Card } from 'semantic-ui-react'
import { RuleProject, RuleProperty } from '../../../app/models/ruleProject';

interface Props {
    ruleProject: RuleProject
}

function renderProperty(property: RuleProperty) {
    return (
        <li key={property.id}>
            Name: {property.name} <br />
            Type: {property.type} <br />
            {property.subProperties && property.subProperties.length > 0 && (
                <>
                    <div>Sub-Properties:</div>
                    <ul>
                        {property.subProperties.map(subProperty => renderProperty(subProperty))}
                    </ul>
                </>
            )}
        </li>
    );
}

export default observer(function RuleProjectDetailedInfo({ ruleProject }: Props) {
    return (
        <Card fluid>
            <Card.Content>
                <Card.Header>{ruleProject.name}</Card.Header>
                <Card.Description>
                    <div>Properties:</div>
                    <ul>
                        {ruleProject.properties.map(property => renderProperty(property))}
                    </ul>
                    <div>Actions:</div>
                    <ul>
                        {ruleProject.standardRules.map(rule => (
                            <li key={rule.id}>
                                Name: {rule.name}
                            </li>
                        ))}
                    </ul>
                </Card.Description>
            </Card.Content>
        </Card>
    )
})
