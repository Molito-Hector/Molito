import { useState } from 'react';
import { Button, Modal, Form, Input, Header, Divider } from 'semantic-ui-react';
import { observer } from 'mobx-react-lite';
import { useStore } from '../../app/stores/store';
import { Formik } from 'formik';
import { OrgFormValues } from '../../app/models/organization';

const OrganizationPanel = () => {
    const { organizationStore } = useStore();
    const { createOrganization, selectedOrganization, updateOrg } = organizationStore;
    const [open, setOpen] = useState(false);
    const [editMode, setEditMode] = useState(false);

    const handleCreateOrganization = async (values: OrgFormValues) => {
        await createOrganization(values);
        setOpen(false);
    };

    const handleUpdateOrganization = async (values: OrgFormValues) => {
        await updateOrg(values);
        setEditMode(false);
    };

    return (
        <div>
            <Header as='h1'>Organization Info</Header>
            <Button floated='right' color='teal' onClick={() => setEditMode(true)}>Edit</Button>
            <Divider />
            {selectedOrganization ? (
                <>
                    {!editMode ? (
                        <>
                            <Form>
                                <Form.Field key="name" width={16}>
                                    <label>Organization Name</label>
                                    <input value={selectedOrganization.name} readOnly />
                                </Form.Field>
                                <Form.Field key="createdAt">
                                    <label>Description</label>
                                    <input
                                        value={selectedOrganization.description}
                                        readOnly
                                    />
                                </Form.Field>
                            </Form>
                        </>
                    ) : (
                        <Formik
                            initialValues={selectedOrganization}
                            onSubmit={handleUpdateOrganization}
                        >
                            {({ handleSubmit, handleChange, values }) => (
                                <Form onSubmit={handleSubmit}>
                                    <Form.Field>
                                        <label>Organization Name</label>
                                        <Input
                                            name='name'
                                            placeholder='Organization Name'
                                            value={values.name}
                                            onChange={handleChange}
                                        />
                                    </Form.Field>
                                    <Form.Field>
                                        <label>Description</label>
                                        <Input
                                            name='description'
                                            placeholder='Description'
                                            value={values.description}
                                            onChange={handleChange}
                                        />
                                    </Form.Field>
                                    <Button onClick={() => setEditMode(false)}>Cancel</Button>
                                    <Button positive type='submit'>Save Changes</Button>
                                </Form>
                            )}
                        </Formik>
                    )}
                </>
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
                    <Formik
                        initialValues={{ name: '', description: '' }}
                        onSubmit={handleCreateOrganization}
                    >
                        {({ handleSubmit, handleChange, values }) => (
                            <Form onSubmit={handleSubmit}>
                                <Form.Field>
                                    <label>Organization Name</label>
                                    <Input
                                        name='name'
                                        placeholder='Organization Name'
                                        value={values.name}
                                        onChange={handleChange}
                                    />
                                </Form.Field>
                                <Form.Field>
                                    <label>Description</label>
                                    <Input
                                        name='description'
                                        placeholder='Description'
                                        value={values.description}
                                        onChange={handleChange}
                                    />
                                </Form.Field>
                                <Button type='submit'>Create</Button>
                            </Form>
                        )}
                    </Formik>
                </Modal.Content>
            </Modal>
        </div>
    );
};

export default observer(OrganizationPanel);