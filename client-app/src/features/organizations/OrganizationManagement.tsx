import React, { useEffect } from 'react';
import { Menu, Segment } from 'semantic-ui-react';
import OrganizationPanel from './OrganizationPanel';
import OrganizationMembersPanel from './OrganizationMembersPanel';
import { observer } from 'mobx-react-lite';
import { useStore } from '../../app/stores/store';
import LoadingComponent from '../../app/layout/LoadingComponent';

const OrganizationManagement = () => {
    const { organizationStore } = useStore();
    const { selectedOrganization : org, loadOrganization, clearSelectedOrganization, loading, loadingInitial } = organizationStore;
    const { userStore: { user } } = useStore();

    const [activeItem, setActiveItem] = React.useState('Organization');

    useEffect(() => {
        if (user?.orgId) loadOrganization(user.orgId);
        return () => clearSelectedOrganization();
    }, [user, loadOrganization, clearSelectedOrganization])

    const handleItemClick = (name: string) => {
        setActiveItem(name);
    };

    if (loadingInitial || !org) return <LoadingComponent />;
    if (loading) return <LoadingComponent content="Saving changes..." />;

    return (
        <Segment>
            <Menu vertical>
                <Menu.Item
                    name='Organization'
                    active={activeItem === 'Organization'}
                    onClick={() => handleItemClick('Organization')}
                />
                <Menu.Item
                    name='Organization Members'
                    active={activeItem === 'Organization Members'}
                    onClick={() => handleItemClick('Organization Members')}
                />
            </Menu>

            {activeItem === 'Organization' && <OrganizationPanel />}
            {activeItem === 'Organization Members' && <OrganizationMembersPanel />}
        </Segment>
    );
};

export default observer(OrganizationManagement);