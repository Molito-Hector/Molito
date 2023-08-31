import { Modal, Button, Header, Icon } from "semantic-ui-react";
import { useStore } from "../../../../app/stores/store";
import { observer } from "mobx-react-lite";

interface Props {
    ruleId: string;
    ruleName: string;
    open: boolean;
    type: string;
    onClose: () => void;
}

export default observer(function DeleteModal({ open, ruleId, ruleName, type, onClose }: Props) {
    const { decisionTableStore, ruleStore } = useStore();
    const { deleteTable, loading: tableLoading } = decisionTableStore;
    const { deleteRule, loading: ruleLoading } = ruleStore;

    const handleCloseModal = () => {
        onClose();
    };

    const handleDeleteTable = async (id: string) => {
        try {
            type === "Decision Table" ? await deleteTable(id) : await deleteRule(id);
            handleCloseModal();
        } catch (error) {
            console.log(error);
        }
    };

    return (
        <Modal
            basic
            onClose={handleCloseModal}
            open={open}
            size='small'
        >
            <Header icon>
                <Icon name='trash alternate outline' />
                Delete Rule
            </Header>
            <Modal.Content>
                <p>
                    Are you sure you want to delete {type} '{ruleName}'? This can't be undone
                </p>
            </Modal.Content>
            <Modal.Actions>
                <Button basic color='red' inverted onClick={handleCloseModal}>
                    <Icon name='remove' /> No
                </Button>
                <Button color='green' loading={type === "Decision Table" ? tableLoading : ruleLoading} inverted onClick={() => handleDeleteTable(ruleId)}>
                    <Icon name='checkmark' /> Yes
                </Button>
            </Modal.Actions>
        </Modal>
    );
})