import { useState } from 'react';
import { Button, Modal, Input, Header, Divider, Table, Icon } from 'semantic-ui-react';
import { observer } from 'mobx-react-lite';
import { useStore } from '../../app/stores/store';
import RemoveMemberModal from '../ruleProjects/details/Tabs/RemoveMemberModal';

const OrganizationMembersPanel = () => {
    const { organizationStore } = useStore();
    const { updateMembership, selectedOrganization, loading } = organizationStore;
    const [open, setOpen] = useState(false);
    const [username, setUsername] = useState('');
    const [deleteModalOpen, setDeleteModalOpen] = useState(false);
    const [memberToRemove, setMemberToRemove] = useState('');

    const handleAddMember = async (username: string) => {
        await updateMembership(username);
        setOpen(false);
    };

    const handleOpenDeleteModal = (member: string) => {
        setMemberToRemove(member);
        setDeleteModalOpen(true);
    };

    const handleCloseModal = () => {
        setDeleteModalOpen(false);
    }

    const handleRemoveMember = async (username: string) => {
        await updateMembership(username);
        setDeleteModalOpen(false);
    };

    return (
        <div>
            <Header as='h1'>Organization Members</Header>
            <Button floated='right' color='teal' onClick={() => setOpen(true)}>Add/Remove Member</Button>
            <Divider />
            <RemoveMemberModal loading={loading} memberName={memberToRemove} onClose={handleCloseModal} onSubmit={handleRemoveMember} open={deleteModalOpen} />

            <Modal
                onClose={() => setOpen(false)}
                onOpen={() => setOpen(true)}
                open={open}
            >
                <Modal.Header>Add/Remove Member</Modal.Header>
                <Modal.Content>
                    <Input
                        placeholder='Username'
                        value={username}
                        onChange={(e) => setUsername(e.target.value)}
                    />
                    <Button onClick={() => handleAddMember(username)}>Add</Button>
                </Modal.Content>
            </Modal>

            <Table celled striped fixed color='purple'>
                <Table.Header>
                    <Table.Row>
                        <Table.HeaderCell>Username</Table.HeaderCell>
                        <Table.HeaderCell>Display Name</Table.HeaderCell>
                        <Table.HeaderCell>Actions</Table.HeaderCell>
                    </Table.Row>
                </Table.Header>
                <Table.Body>
                    {(selectedOrganization?.members || []).map((member) => (
                        <Table.Row key={member.username}>
                            <Table.Cell>{member.username}</Table.Cell>
                            <Table.Cell>{member.displayName}</Table.Cell>
                            <Table.Cell>
                                <Icon
                                    name='trash'
                                    style={{ opacity: 0.3, transition: 'opacity 0.2s', cursor: 'pointer' }}
                                    className="deletable-column"
                                    onClick={() => handleOpenDeleteModal(member.username)}
                                />
                            </Table.Cell>
                        </Table.Row>
                    ))}
                </Table.Body>
            </Table>
        </div>
    );
};

export default observer(OrganizationMembersPanel);