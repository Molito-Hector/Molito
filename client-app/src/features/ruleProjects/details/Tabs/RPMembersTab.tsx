import { Segment, Button, Modal, Table, Icon, Form } from "semantic-ui-react";
import { RuleProject } from '../../../../app/models/ruleProject';
import { observer } from "mobx-react-lite";
import { useStore } from "../../../../app/stores/store";
import { useEffect, useRef, useState } from "react";
import '../../../../app/layout/styles.css';
import RemoveMemberModal from "./RemoveMemberModal";

interface Props {
    ruleProject: RuleProject;
}

export default observer(function GeneralTab({ ruleProject }: Props) {
    const { ruleProjectStore, organizationStore } = useStore();
    const { updateMembership, loading } = ruleProjectStore;
    const { selectedOrganization: organization, loadOrganization, clearSelectedOrganization } = organizationStore;
    const { userStore: { user } } = useStore();
    const [open, setOpen] = useState(false);
    const [deleteModalOpen, setDeleteModalOpen] = useState(false);
    const [username, setUsername] = useState('');
    const [memberToRemove, setMemberToRemove] = useState('');

    const handleAddMember = async (username: string) => {
        await updateMembership(username);
        setOpen(false);
    };

    useEffect(() => {
        if (user?.orgId && user?.orgId !== "00000000-0000-0000-0000-000000000000") loadOrganization(user.orgId);
        return () => clearSelectedOrganization();
    }, [user, loadOrganization, clearSelectedOrganization])

    const handleOpenDeleteModal = (member: string) => {
        setMemberToRemove(member);
        setDeleteModalOpen(true);
    };

    const handleRemoveMember = async (username: string) => {
        await updateMembership(username);
        setDeleteModalOpen(false);
    };

    const handleCloseModal = () => {
        setDeleteModalOpen(false);
    }

    const isInitialOpen = useRef(true);

    useEffect(() => {
        if (open && isInitialOpen.current && organization!.members.length > 0) {
            setUsername(organization!.members[0].username);
            isInitialOpen.current = false;
        }

        if (!open) {
            isInitialOpen.current = true;
        }
    }, [open, organization]);

    return (
        <Segment clearing raised>
            <RemoveMemberModal loading={loading} memberName={memberToRemove} onClose={handleCloseModal} onSubmit={handleRemoveMember} open={deleteModalOpen} />
            <Button floated="right" color="teal" onClick={() => setOpen(true)}>Add/Remove Member</Button>
            <Modal
                onClose={() => setOpen(false)}
                onOpen={() => setOpen(true)}
                open={open}
            >
                <Modal.Header>Add/Remove Member</Modal.Header>
                <Modal.Content>
                    <Form>
                        <Form.Field>
                            <label>User</label>
                            <select value={username} onChange={(e) => {
                                setUsername(e.target.value);
                            }}>
                                {organization?.members.map(member => (
                                    <option key={member.username} value={member.username}>
                                        {member.displayName}
                                    </option>
                                ))}
                            </select>
                        </Form.Field>
                    </Form>
                    <Button positive floated='right' onClick={() => handleAddMember(username)}>Add</Button>
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
                    {ruleProject?.members.map((member) => (
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
        </Segment>
    );
})