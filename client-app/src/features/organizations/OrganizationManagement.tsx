import { useEffect } from 'react';
import { Grid, Segment } from 'semantic-ui-react';
import OrganizationPanel from './OrganizationPanel';
import OrganizationMembersPanel from './OrganizationMembersPanel';
import { observer } from 'mobx-react-lite';
import { useStore } from '../../app/stores/store';
import LoadingComponent from '../../app/layout/LoadingComponent';

const OrganizationManagement = () => {
    const { organizationStore } = useStore();
    const { selectedOrganization: org, loadOrganization, clearSelectedOrganization, loading, loadingInitial } = organizationStore;
    const { userStore: { user } } = useStore();

    useEffect(() => {
        if (user?.orgId) loadOrganization(user.orgId);
        return () => clearSelectedOrganization();
    }, [user, loadOrganization, clearSelectedOrganization])

    if (loadingInitial || !org) return <LoadingComponent />;
    if (loading) return <LoadingComponent content="Saving changes..." />;

    return (
        <Segment raised clearing>
            <Grid>
                <Grid.Column width={6}>
                    <Segment raised clearing>
                        <OrganizationPanel />
                    </Segment>
                </Grid.Column>
                <Grid.Column width={10}>
                    <Segment raised clearing>
                        <OrganizationMembersPanel />
                    </Segment>
                </Grid.Column>
            </Grid>
        </Segment>
    );
};

export default observer(OrganizationManagement);