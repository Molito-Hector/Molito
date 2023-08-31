import { useStore } from '../../../app/stores/store';
import { observer } from 'mobx-react-lite';
import { Button, Grid, Header, Icon, Segment, Table } from 'semantic-ui-react';
import { Link } from 'react-router-dom';
import { format } from 'date-fns';
import { useState } from 'react';
import CreateRPModal from './CreateRPModal';
import { RuleProject } from '../../../app/models/ruleProject';
import DeleteRPModal from './DeleteRPModal';

export default observer(function ActivityList() {
    const { ruleProjectStore } = useStore();
    const { ruleProjectsByName } = ruleProjectStore;
    const [modalOpen, setModalOpen] = useState(false);

    const handleOpenModal = () => {
        setModalOpen(true);
    };

    const handleCloseModal = () => {
        setModalOpen(false);
    };

    const [RPToDelete, setRPToDelete] = useState<RuleProject | null>(null);
    const [deleteModalOpen, setDeleteModalOpen] = useState(false);

    const handleOpenDeleteModal = (ruleProject: RuleProject) => {
        setRPToDelete(ruleProject);
        setDeleteModalOpen(true);
    };

    const handleCloseDeleteModal = () => {
        setDeleteModalOpen(false);
    };

    function truncate(str: string | undefined) {
        if (str) {
            return str.length > 40 ? str.substring(0, 37) + '...' : str;
        }
    }

    return (
        <Segment clearing>
            <Grid>
                <Grid.Row>
                    <Grid.Column width={8}>
                        <Header size='huge' content='Decision Library' sub color='teal' />
                    </Grid.Column>
                    <Grid.Column width={8}>
                        <Button onClick={handleOpenModal} floated='right' color='teal' content='New Rule Project' />
                        <CreateRPModal open={modalOpen} onClose={handleCloseModal} />
                    </Grid.Column>
                </Grid.Row>
            </Grid>
            <Table celled selectable>
                <Table.Header>
                    <Table.Row>
                        <Table.HeaderCell>Rule Project</Table.HeaderCell>
                        <Table.HeaderCell>Description</Table.HeaderCell>
                        <Table.HeaderCell>Created</Table.HeaderCell>
                        <Table.HeaderCell>Actions</Table.HeaderCell>
                    </Table.Row>
                </Table.Header>
                <Table.Body>
                    {ruleProjectsByName.map((ruleproject) => (
                        <Table.Row key={ruleproject.id}>
                            <Table.Cell>
                                <Link to={`/ruleprojects/${ruleproject.id}`}>
                                    {ruleproject.name}
                                </Link>
                            </Table.Cell>
                            <Table.Cell>{truncate(ruleproject.description)}</Table.Cell>
                            <Table.Cell>{format(ruleproject.createdAt, 'MM/dd/yyyy')}</Table.Cell>
                            <Table.Cell>
                                <Icon name="edit" />
                                <Icon
                                    name="trash"
                                    onClick={() => handleOpenDeleteModal(ruleproject)}
                                    style={{ cursor: 'pointer' }}
                                />
                            </Table.Cell>
                        </Table.Row>
                    ))}
                </Table.Body>
            </Table>
            <DeleteRPModal open={deleteModalOpen} onClose={handleCloseDeleteModal} ruleProjectId={RPToDelete?.id || ''} ruleProjectName={RPToDelete?.name || ''} />
        </Segment>
    )
})