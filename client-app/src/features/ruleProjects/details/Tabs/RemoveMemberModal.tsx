import { Modal, Button, Header, Icon } from "semantic-ui-react";
import { observer } from "mobx-react-lite";

interface Props {
    memberName: string;
    open: boolean;
    loading: boolean;
    onClose: () => void;
    onSubmit: (username: string) => void;
}

export default observer(function RemoveMemberModal({ open, loading, memberName, onClose, onSubmit }: Props) {

    const handleSubmit = () => {
        onSubmit(memberName);
        onClose();
    };

    return (
        <Modal
            basic
            onClose={onClose}
            open={open}
            size='small'
        >
            <Header icon>
                <Icon name='trash alternate outline' />
                Delete Rule
            </Header>
            <Modal.Content>
                <p>
                    Are you sure you want to remove member {memberName} from the project? They will lose access untill re-added
                </p>
            </Modal.Content>
            <Modal.Actions>
                <Button basic color='red' inverted onClick={onClose}>
                    <Icon name='remove' /> No
                </Button>
                <Button color='green' loading={loading} inverted onClick={handleSubmit}>
                    <Icon name='checkmark' /> Yes
                </Button>
            </Modal.Actions>
        </Modal>
    );
})