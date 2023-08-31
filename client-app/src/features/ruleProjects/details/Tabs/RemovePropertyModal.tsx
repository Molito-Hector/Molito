import { Modal, Button, Header, Icon } from "semantic-ui-react";
import { useStore } from "../../../../app/stores/store";
import { observer } from "mobx-react-lite";

interface Props {
    propId: string;
    propName: string;
    open: boolean;
    onClose: () => void;
}

export default observer(function RemovePropertyModal({ open, propId, propName, onClose }: Props) {
    const { ruleProjectStore } = useStore();
    const { removeProperty, loading } = ruleProjectStore;

    const handleCloseModal = () => {
        onClose();
    };

    const handleDeleteTable = async (id: string) => {
        try {
            await removeProperty(id);
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
                    Are you sure you want to delete property {propName}? This can't be undone
                </p>
            </Modal.Content>
            <Modal.Actions>
                <Button basic color='red' inverted onClick={handleCloseModal}>
                    <Icon name='remove' /> No
                </Button>
                <Button color='green' loading={loading} inverted onClick={() => handleDeleteTable(propId)}>
                    <Icon name='checkmark' /> Yes
                </Button>
            </Modal.Actions>
        </Modal>
    );
})