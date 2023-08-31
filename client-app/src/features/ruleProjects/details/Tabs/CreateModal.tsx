import { useState } from "react";
import { Modal, Button, Segment } from "semantic-ui-react";
import { ErrorMessage, Field, FieldProps, Form, Formik } from "formik";
import * as Yup from 'yup';
import { DTFormValues } from "../../../../app/models/decisionTable";
import { v4 as uuid } from 'uuid';
import { useStore } from "../../../../app/stores/store";
import { RuleFormValues } from "../../../../app/models/rule";

interface Props {
    ruleProjectId: string;
    open: boolean;
    type: string;
    onClose: () => void;
}

export default function CreateModal({ open, ruleProjectId, type, onClose }: Props) {
    const { decisionTableStore, ruleStore } = useStore();
    const { createTable } = decisionTableStore;
    const { createRule } = ruleStore;
    const [table] = useState<DTFormValues>(new DTFormValues());
    const [rule] = useState<RuleFormValues>(new RuleFormValues());

    const handleCloseModal = () => {
        onClose();
    };

    const handleCreateTable = async <T extends DTFormValues | RuleFormValues>(values: T) => {
        try {
            if (type === "Decision Table") {
                const newTable = {
                    ...values,
                    id: uuid(),
                    evaluationType: "MultiHit",
                    ruleProjectId: ruleProjectId ?? '',
                };
                await createTable(newTable as DTFormValues);
            } else {
                const newRule = {
                    ...values,
                    id: uuid(),
                    ruleProjectId: ruleProjectId ?? '',
                };
                await createRule(newRule);
            }

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
            <Modal.Header>Create {type}</Modal.Header>
            <Modal.Content>
                <Segment clearing>
                    <Formik
                        initialValues={type === "Decision Table" ? table : rule}
                        validationSchema={validationSchema}
                        onSubmit={(values) => {
                            handleCreateTable(values);
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