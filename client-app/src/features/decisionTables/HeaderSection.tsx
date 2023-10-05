import { Button, Header, Icon } from "semantic-ui-react";

interface Props {
    editMode: boolean;
    setEditMode: (mode: boolean) => void;
    addRow: () => void;
    handleSaveChanges: () => void;
    setIsAddConditionModalOpen: (state: boolean) => void;
}

export const HeaderSection: React.FC<Props> = ({ editMode, setEditMode, addRow, handleSaveChanges, setIsAddConditionModalOpen }) => {
    return (
        <Header as='h3' dividing>
            <Icon inverted color='purple' name='table' />Decision Table Details
            <Button floated="right" color='teal' onClick={() => window.history.back()}>Back</Button>
            <Button onClick={() => setEditMode(!editMode)}>
                {editMode ? 'Cancel Edit' : 'Edit Mode'}
            </Button>
            {editMode && (
                <>
                    <Button onClick={addRow}>Add Row</Button>
                    <Button onClick={handleSaveChanges}>Save Changes</Button>
                    <Button onClick={() => setIsAddConditionModalOpen(true)}>Add Condition</Button>
                </>
            )}
        </Header>
    );
};