import { useState } from 'react';
import { Button, Modal, Form, Input } from 'semantic-ui-react';
import { observer } from 'mobx-react-lite';
import { useStore } from '../../app/stores/store';

const OrganizationPanel = () => {
    const { organizationStore } = useStore();
    const { createOrganization, selectedOrganization } = organizationStore;
    const [open, setOpen] = useState(false);
    const [orgName, setOrgName] = useState('');

    const handleCreateOrganization = async () => {
        await createOrganization({
            name: orgName,
            description: 'Molito'
        });
        setOpen(false);
    };

    return (
        <div>
            <h2>Organization Info</h2>
            {selectedOrganization ? (
                <p>{selectedOrganization.name}</p>
            ) : (
                <Button onClick={() => setOpen(true)}>Create Organization</Button>
            )}

            <Modal
                onClose={() => setOpen(false)}
                onOpen={() => setOpen(true)}
                open={open}
            >
                <Modal.Header>Create Organization</Modal.Header>
                <Modal.Content>
                    <Form onSubmit={handleCreateOrganization}>
                        <Form.Field>
                            <label>Organization Name</label>
                            <Input
                                placeholder='Organization Name'
                                value={orgName}
                                onChange={(e) => setOrgName(e.target.value)}
                            />
                        </Form.Field>
                        <Button type='submit'>Create</Button>
                    </Form>
                </Modal.Content>
            </Modal>
        </div>
    );
};

export default observer(OrganizationPanel);