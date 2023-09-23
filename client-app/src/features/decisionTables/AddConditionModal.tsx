import { useState } from "react";
import { Button, Form, Modal } from "semantic-ui-react";

interface Props {
    open: boolean;
    onClose: () => void;
    onSubmit: (field: string, operator: string) => void;
}

export default function AddConditionModal({ open, onClose, onSubmit }: Props) {
    const [field, setField] = useState('');
    const [operator, setOperator] = useState('');

    const handleSubmit = () => {
        onSubmit(field, operator);
        onClose();
    };

    return (
        <Modal open={open} onClose={onClose}>
            <Modal.Header>Add New Condition</Modal.Header>
            <Modal.Content>
                <Form>
                    <Form.Field>
                        <label>Field</label>
                        <input placeholder='Field' value={field} onChange={(e) => setField(e.target.value)} />
                    </Form.Field>
                    <Form.Field>
                        <label>Operator</label>
                        <input placeholder='Operator' value={operator} onChange={(e) => setOperator(e.target.value)} />
                    </Form.Field>
                </Form>
            </Modal.Content>
            <Modal.Actions>
                <Button onClick={onClose}>Cancel</Button>
                <Button primary onClick={handleSubmit}>Add Condition</Button>
            </Modal.Actions>
        </Modal>
    );
}