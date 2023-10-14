import { Icon, Segment, Button, List, Header, Divider } from "semantic-ui-react";
import { RuleProject, RuleProperty } from "../../../../app/models/ruleProject";
import { observer } from "mobx-react-lite";
import { useState } from "react";
import AddPropertyModal from "./AddPropertyModal";
import { useStore } from "../../../../app/stores/store";
import { v4 as uuid } from 'uuid';
import RemovePropertyModal from "./RemovePropertyModal";
import '../../../../app/layout/styles.css';


interface Props {
    ruleProject: RuleProject;
}

export default observer(function DataModelTab({ ruleProject }: Props) {
    const { ruleProjectStore } = useStore();

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

    return (
        <>
            <Button floated="right" color='teal' onClick={() => setModalOpen(true)}>Add Property</Button>
            <AddPropertyModal
                open={modalOpen}
                onClose={() => setModalOpen(false)}
                onSubmit={handleAddProperty}
            />
            <RemovePropertyModal open={deleteModalOpen} onClose={handleCloseDeleteModal} propId={propToDelete?.id || ''} propName={propToDelete?.name || ''} />
            <Header as='h3'>
                <Icon name='arrow down' />
                <Header.Content>Input Properties</Header.Content>
            </Header>
            <Divider />
            <Segment clearing raised>
                <List divided relaxed>
                    {(ruleProject.properties || []).filter(p => p.direction === "I").map((property, index) => (
                        <List.Item key={property.id}>
                            <Icon name="trash" color="grey" onClick={() => handleOpenDeleteModal(property)} style={{ float: 'right', opacity: 0.3, transition: 'opacity 0.2s', cursor: 'pointer' }} className="deletable-column" />
                            <List.Content>
                                <List.Header onClick={() => handleClick(index)}>
                                    {property.name}
                                </List.Header>
                                <List.Description>
                                    {property.type}
                                </List.Description>
                                {activeIndex === index && property.subProperties && property.subProperties.length > 0 && (
                                    <List.List>
                                        {property.subProperties.map(subProperty => (
                                            <List.Item key={subProperty.id}>
                                                <List.Content>
                                                    <List.Header>
                                                        {subProperty.name}
                                                    </List.Header>
                                                    <List.Description>{subProperty.type}</List.Description>
                                                </List.Content>
                                            </List.Item>
                                        ))}
                                    </List.List>
                                )}
                            </List.Content>
                        </List.Item>
                    ))}
                </List>
            </Segment>
            <Header as='h3'>
                <Icon name='arrow up' />
                <Header.Content>Output Properties</Header.Content>
            </Header>
            <Divider />
            <Segment clearing raised>
                <List divided relaxed>
                    {ruleProject.properties.filter(p => p.direction === "O").map((property, index) => (
                        <List.Item key={property.id}>
                            <Icon name="trash" color="grey" onClick={() => handleOpenDeleteModal(property)} style={{ float: 'right', opacity: 0.3, transition: 'opacity 0.2s', cursor: 'pointer' }} className="deletable-column" />
                            <List.Content>
                                <List.Header onClick={() => handleClick(index + 100)}>
                                    {property.name}
                                </List.Header>
                                <List.Description>
                                    {property.type}
                                </List.Description>
                                {activeIndex === index + 100 && property.subProperties && property.subProperties.length > 0 && (
                                    <List.List>
                                        {property.subProperties.map(subProperty => (
                                            <List.Item key={subProperty.id}>
                                                <List.Content>
                                                    <List.Header>
                                                        {subProperty.name}
                                                    </List.Header>
                                                    <List.Description>{subProperty.type}</List.Description>
                                                </List.Content>
                                            </List.Item>
                                        ))}
                                    </List.List>
                                )}
                            </List.Content>
                        </List.Item>
                    ))}
                </List>
            </Segment>
            <Header as='h3'>
                <Icon name='arrows alternate vertical' />
                <Header.Content>Bidirectional Properties</Header.Content>
            </Header>
            <Divider />
            <Segment clearing raised>
                <List divided relaxed>
                    {ruleProject.properties.filter(p => p.direction === "B").map((property, index) => (
                        <List.Item key={property.id}>
                            <Icon name="trash" color="grey" onClick={() => handleOpenDeleteModal(property)} style={{ float: 'right', opacity: 0.3, transition: 'opacity 0.2s', cursor: 'pointer' }} className="deletable-column" />
                            <List.Content>
                                <List.Header onClick={() => handleClick(index + 200)}>
                                    {property.name}
                                </List.Header>
                                <List.Description>
                                    {property.type}
                                </List.Description>
                                {activeIndex === index + 200 && property.subProperties && property.subProperties.length > 0 && (
                                    <List.List>
                                        {property.subProperties.map(subProperty => (
                                            <List.Item key={subProperty.id}>
                                                <List.Content>
                                                    <List.Header>
                                                        {subProperty.name}
                                                    </List.Header>
                                                    <List.Description>{subProperty.type}</List.Description>
                                                </List.Content>
                                            </List.Item>
                                        ))}
                                    </List.List>
                                )}
                            </List.Content>
                        </List.Item>
                    ))}
                </List>
            </Segment>
        </>
    );
})