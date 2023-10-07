import { Grid, Form, Segment, List, Button, Modal, Input } from "semantic-ui-react";
import { RuleProject } from '../../../../app/models/ruleProject';
import { observer } from "mobx-react-lite";
import { useStore } from "../../../../app/stores/store";
import { useState } from "react";

interface Props {
    ruleProject: RuleProject;
}

export default observer(function GeneralTab({ ruleProject }: Props) {
    const { ruleProjectStore } = useStore();
    const { updateMembership } = ruleProjectStore;
    const [open, setOpen] = useState(false);
    const [username, setUsername] = useState('');

    const handleAddMember = async (username: string) => {
        await updateMembership(username);
        setOpen(false);
    };

    const handleRemoveMember = async (username: string) => {
        await updateMembership(username);
    };

    return (
        <Segment clearing raised>
            <Button onClick={() => setOpen(true)}>Add/Remove Member</Button>
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
            <List>
                {ruleProject?.members.map((member) => (
                    <List.Item key={member.username}>
                        <List.Content>
                            <List.Header>{member.username}</List.Header>
                            {member.displayName}
                            <Button onClick={() => handleRemoveMember(member.username)}>Remove</Button>
                        </List.Content>
                    </List.Item>
                ))}
            </List>
        </Segment>
    );
})