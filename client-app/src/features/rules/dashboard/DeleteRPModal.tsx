import { Modal, Button, Header, Icon } from "semantic-ui-react";
import { observer } from "mobx-react-lite";
import { useStore } from "../../../app/stores/store";

interface Props {
    ruleProjectId: string;
    ruleProjectName: string;
    open: boolean;
    onClose: () => void;
}

export default observer(function DeleteRPModal({ open, ruleProjectId, ruleProjectName, onClose }: Props) {
    const { ruleProjectStore } = useStore();
    const { deleteRuleProject, loading } = ruleProjectStore;

    const handleCloseModal = () => {
        onClose();
    };

    const handleDeleteProject = async (id: string) => {
        try {
            await deleteRuleProject(id);
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
                Delete Rule Project
            </Header>
            <Modal.Content>
                <p>
                    Are you sure you want to delete {ruleProjectName}? The rule project and all related decision tables, rules and other objects will be deleted. This action can't be undone
                </p>
            </Modal.Content>
            <Modal.Actions>
                <Button basic color='red' inverted onClick={handleCloseModal}>
                    <Icon name='remove' /> No
                </Button>
                <Button color='green' loading={loading} inverted onClick={() => handleDeleteProject(ruleProjectId)}>
                    <Icon name='checkmark' /> Yes
                </Button>
            </Modal.Actions>
        </Modal>
    );
})