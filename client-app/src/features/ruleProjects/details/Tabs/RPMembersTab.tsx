import { Segment, Button, Modal, Input, Table, Icon } from "semantic-ui-react";
import { RuleProject } from '../../../../app/models/ruleProject';
import { observer } from "mobx-react-lite";
import { useStore } from "../../../../app/stores/store";
import { useState } from "react";
import '../../../../app/layout/styles.css';
import RemoveMemberModal from "./RemoveMemberModal";

interface Props {
    ruleProject: RuleProject;
}

export default observer(function GeneralTab({ ruleProject }: Props) {
    const { ruleProjectStore } = useStore();
    const { updateMembership, loading } = ruleProjectStore;
    const [open, setOpen] = useState(false);
    const [deleteModalOpen, setDeleteModalOpen] = useState(false);
    const [username, setUsername] = useState('');
    const [memberToRemove, setMemberToRemove] = useState('');

    const handleAddMember = async (username: string) => {
        await updateMembership(username);
        setOpen(false);
    };

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