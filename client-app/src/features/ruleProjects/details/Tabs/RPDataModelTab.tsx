import { Grid, Accordion, Icon, Segment, Button } from "semantic-ui-react";
import { RuleProject, RuleProperty } from "../../../../app/models/ruleProject";
import { observer } from "mobx-react-lite";
import { useState } from "react";
import AddPropertyModal from "./AddPropertyModal";
import { useStore } from "../../../../app/stores/store";
import { v4 as uuid } from 'uuid';
import RemovePropertyModal from "./RemovePropertyModal";


interface Props {
    ruleProject: RuleProject;
}

export default observer(function DataModelTab({ ruleProject }: Props) {
    const { ruleProjectStore } = useStore();
    const getDirectionLabel = (direction: string) => {
        switch (direction) {
            case "I":
                return "Input";
            case "O":
                return "Output";
            case "B":
                return "Bidirectional";
            default:
                return "";
        }
    };

    const [propToDelete, setPropToDelete] = useState<RuleProperty | null>(null);
    const [deleteModalOpen, setDeleteModalOpen] = useState(false);

    const handleOpenDeleteModal = (prop: RuleProperty) => {
        setPropToDelete(prop);
        setDeleteModalOpen(true);
    };

    const handleCloseDeleteModal = () => {
        setDeleteModalOpen(false);
    };

    const [activeIndex, setActiveIndex] = useState(-1);
    const [modalOpen, setModalOpen] = useState(false);

    const handleClick = (index: number) => {
        const newIndex = activeIndex === index ? -1 : index;
        setActiveIndex(newIndex);
    };

    const handleAddProperty = async (property: RuleProperty) => {
        property.id = uuid();
        await ruleProjectStore.addProperties([property]);
        setModalOpen(false);
    };

    const renderProperty = (property: RuleProperty, index: number) => {
        const directionLabel = getDirectionLabel(property.direction);
        const title = `${property.name} (${property.type}) - ${directionLabel}`;
        if (property.subProperties && property.subProperties.length > 0) {
            return (
                <>
                    <Accordion key={property.id}>
                        <Accordion.Title
                            active={activeIndex === index}
                            index={index}
                            onClick={() => handleClick(index)}
                        >
                            <Icon name="dropdown" />
                            {title}
                            <Icon
                                name="trash"
                                onClick={() => handleOpenDeleteModal(property)}
                                style={{ cursor: 'pointer' }}
                            />
                        </Accordion.Title>
                        <Accordion.Content active={activeIndex === index}>
                            {property.subProperties.map((subProperty, i) =>
                                renderProperty(subProperty, i)
                            )}
                        </Accordion.Content>
                    </Accordion>
                </>
            );
        } else {
            return (
                <>
                    <p key={property.id}>{title}</p>
                    <Icon
                        name="trash"
                        onClick={() => handleOpenDeleteModal(property)}
                        style={{ cursor: 'pointer' }}
                    />
                </>
            )
        }
    };

    return (
        <Grid>
            <Grid.Column width={16}>
                <Segment raised>
                    {ruleProject.properties.map((property, index) =>
                        renderProperty(property, index)
                    )}
                    <Button onClick={() => setModalOpen(true)}>Add Property</Button>
                    <AddPropertyModal
                        open={modalOpen}
                        onClose={() => setModalOpen(false)}
                        onSubmit={handleAddProperty}
                    />
                    <RemovePropertyModal open={deleteModalOpen} onClose={handleCloseDeleteModal} propId={propToDelete?.id || ''} propName={propToDelete?.name || ''} />
                </Segment>
            </Grid.Column>
        </Grid>
    );
})