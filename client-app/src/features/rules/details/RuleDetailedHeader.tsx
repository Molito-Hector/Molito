import { Tab } from 'semantic-ui-react'

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
