import { useEffect, useRef, useState } from "react";
import { Button, Form, Modal } from "semantic-ui-react";
import { RuleProperty } from "../../app/models/ruleProject";
import { filterProperties } from "../../app/helpers/decisionTableHelpers";

interface Props {
    open: boolean;
    onClose: () => void;
    onSubmit: (field: string, operator: string) => void;
    ruleProperties: RuleProperty[];
}

export default function AddConditionModal({ open, ruleProperties, onClose, onSubmit }: Props) {
    const [field, setField] = useState('');
    const [operator, setOperator] = useState('==');
    const filteredProps = filterProperties(ruleProperties, 'Input');

    const handleSubmit = () => {
        onSubmit(field, operator);
        onClose();
    };

    const isInitialOpen = useRef(true);

    useEffect(() => {
        if (open && isInitialOpen.current && filteredProps.length > 0) {
            setField(filteredProps[0].name);
            isInitialOpen.current = false;
        }

        if (!open) {
            isInitialOpen.current = true;
        }
    }, [open, filteredProps]);

    const OPERATORS = [
        { value: ">", label: ">" },
        { value: "<", label: "<" },
        { value: ">=", label: ">=" },
        { value: "<=", label: "<=" },
        { value: "==", label: "==" },
        { value: "!=", label: "!=" },
    ];

    return (
        <Modal open={open} onClose={onClose}>
            <Modal.Header>Add New Condition</Modal.Header>
            <Modal.Content>
                <Form>
                    <Form.Field>
                        <label>Field</label>
                        <select value={field} onChange={(e) => {
                            setField(e.target.value);
                        }}>
                            {filteredProps.map(prop => (
                                <option key={prop.id} value={prop.name}>
                                    {prop.name}
                                </option>
                            ))}
                        </select>
                    </Form.Field>
                    <Form.Field>
                        <label>Operator</label>
                        <select value={operator} onChange={(e) => setOperator(e.target.value)}>
                            {OPERATORS.map(op => (
                                <option key={op.value} value={op.value}>
                                    {op.label}
                                </option>
                            ))}
                        </select>
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