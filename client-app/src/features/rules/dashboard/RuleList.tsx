import { useStore } from '../../../app/stores/store';
import { observer } from 'mobx-react-lite';
import RuleListItem from './RuleListItem';
import { Item } from 'semantic-ui-react';

export default observer(function ActivityList() {
    const { ruleStore } = useStore();
    const { rulesByName } = ruleStore;

    return (
        <>
            <Item.Group divided>
                {rulesByName.map((rule) => (
                    <RuleListItem key={rule.id} rule={rule} />
                ))}
            </Item.Group>
        </>
    )
})