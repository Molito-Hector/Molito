import { Modal, Button, Header, Icon } from "semantic-ui-react";
import { observer } from "mobx-react-lite";

interface Props {
    condId: string;
    type: string;
    open: boolean;
    loading: boolean;
    onSubmit: (condId: string, type: string) => void;
    onClose: () => void;
}

export default observer(function DeleteConditionModal({ open, condId, onClose, onSubmit, loading, type }: Props) {

    const handleCloseModal = () => {
        onClose();
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
                    Are you sure you want to delete the column? This can't be undone
                </p>
            </Modal.Content>
            <Modal.Actions>
                <Button basic color='red' inverted onClick={handleCloseModal}>
                    <Icon name='remove' /> No
                </Button>
                <Button color='green' loading={loading} inverted onClick={() => onSubmit(condId, type)}>
                    <Icon name='checkmark' /> Yes
                </Button>
            </Modal.Actions>
        </Modal>
    );
})