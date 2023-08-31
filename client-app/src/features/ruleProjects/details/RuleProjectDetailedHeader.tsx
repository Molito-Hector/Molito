import { observer } from 'mobx-react-lite';
import { Header, Item, Segment, Image, Tab } from 'semantic-ui-react'
import { Link } from 'react-router-dom';
import { Rule } from '../../../app/models/rule';
import { RuleProject } from '../../../app/models/ruleProject';

const ruleImageStyle = {
    filter: 'brightness(30%)',
};

const ruleImageTextStyle = {
    position: 'absolute',
    bottom: '5%',
    left: '5%',
    width: '100%',
    height: 'auto',
    color: 'white'
};

interface Props {
    ruleProject: RuleProject
}

const panes = [
    {
        menuItem: 'General',
        render: () => <Tab.Pane attached={false}>Rule Info</Tab.Pane>,
    },
    {
        menuItem: 'Rule Definition',
        render: () => <Tab.Pane attached={false}>Tab 2 Content</Tab.Pane>,
    },
    {
        menuItem: 'AI Training',
        render: () => <Tab.Pane attached={false}>Tab 3 Content</Tab.Pane>,
    },
    {
        menuItem: 'Testing',
        render: () => <Tab.Pane attached={false}>Tab 3 Content</Tab.Pane>,
    },
    {
        menuItem: 'Versions',
        render: () => <Tab.Pane attached={false}>Tab 3 Content</Tab.Pane>,
    },
    {
        menuItem: 'Deployment',
        render: () => <Tab.Pane attached={false}>Tab 3 Content</Tab.Pane>,
    },
]

const TabExampleAttachedFalse = () => (
    <Tab menu={{ attached: false }} panes={panes} />
)


export default TabExampleAttachedFalse
