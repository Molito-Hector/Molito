import { useEffect, useRef, useState } from "react";
import { Button, Form, Modal } from "semantic-ui-react";
import { RuleProperty } from "../../app/models/ruleProject";
import { filterProperties } from "../../app/helpers/decisionTableHelpers";

interface Props {
    open: boolean;
    onClose: () => void;
    onSubmit: (targetProperty: string, modificationType: string) => void;
    ruleProperties: RuleProperty[];
}

export default function ActionModal({ open, onClose, onSubmit, ruleProperties }: Props) {
    const [targetProperty, setTargetProperty] = useState('');
    const [modificationType, setModificationType] = useState('Set');
    const filteredProps = filterProperties(ruleProperties, 'Output');


    const handleSubmit = () => {
        onSubmit(targetProperty, modificationType);
        onClose();
    };

    const isInitialOpen = useRef(true);

    useEffect(() => {
        if (open && isInitialOpen.current && filteredProps.length > 0) {
            setTargetProperty(filteredProps[0].name);
            isInitialOpen.current = false;
        }

        if (!open) {
            isInitialOpen.current = true;
        }
    }, [open, filteredProps]);

    const MODTYPES = [
        { value: "Add", label: "Add" },
        { value: "Subtract", label: "Subtract" },
        { value: "Multiply", label: "Multiply" },
        { value: "Divide", label: "Divide" },
        { value: "Append", label: "Append" },
        { value: "Prepend", label: "Prepend" },
        { value: "Set", label: "Set" },
        { value: "Expression", label: "Expression" }
    ];

    return (
        <Modal open={open} onClose={onClose}>
            <Modal.Header>Edit Action</Modal.Header>
            <Modal.Content>
                <Form>
                    <Form.Field>
                        <label>Target Property</label>
                        <select value={targetProperty} onChange={(e) => setTargetProperty(e.target.value)}>
                            {filteredProps.map(prop => (
                                <option key={prop.id} value={prop.name}>
                                    {prop.name}
                                </option>
                            ))}
                        </select>
                    </Form.Field>
                    <Form.Field>
                        <label>Modification Type</label>
                        <select value={modificationType} onChange={(e) => setModificationType(e.target.value)}>
                            {MODTYPES.map(mt => (
                                <option key={mt.value} value={mt.value}>
                                    {mt.label}
                                </option>
                            ))}
                        </select>
                    </Form.Field>
                </Form>
            </Modal.Content>
            <Modal.Actions>
                <Button onClick={onClose}>Cancel</Button>
                <Button primary onClick={handleSubmit}>Save Action</Button>
            </Modal.Actions>
        </Modal>
    );
}