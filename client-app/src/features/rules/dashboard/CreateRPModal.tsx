import { useState } from "react";
import { Modal, Button, Segment } from "semantic-ui-react";
import { ErrorMessage, Field, FieldProps, Form, Formik } from "formik";
import * as Yup from 'yup';
import { v4 as uuid } from 'uuid';
import { useStore } from "../../../app/stores/store";
import { RPFormValues } from "../../../app/models/ruleProject";
import { useNavigate } from "react-router-dom";

interface Props {
    open: boolean;
    onClose: () => void;
}

export default function CreateRPModal({ open, onClose }: Props) {
    const { ruleProjectStore } = useStore();
    const { createRuleProject } = ruleProjectStore;
    const [project] = useState<RPFormValues>(new RPFormValues());
    const navigate = useNavigate();

    const handleCloseModal = () => {
        onClose();
    };

    const handleCreateProject = async (values: RPFormValues) => {
        try {
            const newProject = {
                ...values,
                id: uuid()
            };
            await createRuleProject(newProject).then(() => navigate(`/ruleprojects/${newProject.id}`));
            handleCloseModal();
        } catch (error) {
            console.log(error);
        }
    };

    const validationSchema = Yup.object({
        name: Yup.string().required("Name is required"),
        description: Yup.string().required("Description is required"),
    });

    return (
        <Modal open={open} onClose={handleCloseModal}>
            <Modal.Header>Create Rule Project</Modal.Header>
            <Modal.Content>
                <Segment clearing>
                    <Formik
                        initialValues={project}
                        validationSchema={validationSchema}
                        onSubmit={(values) => {
                            handleCreateProject(values);
                        }}
                    >
                        {({ handleSubmit, isSubmitting, dirty, isValid }) => (
                            <Form className="ui form" onSubmit={handleSubmit} autoComplete="off">
                                <Field name="name">
                                    {({ field, form }: FieldProps) => (
                                        <div className={`field ${form.errors.name && form.touched.name ? 'error' : ''}`}>
                                            <label>Name</label>
                                            <input type="text" {...field} placeholder="Name" />
                                            <ErrorMessage name="name" render={(error: string) => <div className="ui pointing red basic label">{error}</div>} />
                                        </div>
                                    )}
                                </Field>
                                <Field name="description">
                                    {({ field, form }: FieldProps) => (
                                        <div className={`field ${form.errors.description && form.touched.description ? 'error' : ''}`}>
                                            <label>Description</label>
                                            <input type="text" {...field} placeholder="Description" />
                                            <ErrorMessage name="description" render={(error: string) => <div className="ui pointing red basic label">{error}</div>} />
                                        </div>
                                    )}
                                </Field>
                                <Button
                                    disabled={!dirty || isSubmitting || !isValid}
                                    loading={isSubmitting}
                                    type="submit"
                                    floated="right"
                                    positive
                                    content="Create"
                                />
                                <Button
                                    onClick={handleCloseModal}
                                    floated="right"
                                    content="Cancel"
                                />
                            </Form>
                        )}
                    </Formik>
                </Segment>
            </Modal.Content>
        </Modal>
    );
}