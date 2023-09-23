import { useState } from "react";
import { Grid, Dropdown } from "semantic-ui-react";
import { RuleProject } from "../../../../app/models/ruleProject";
import { RulesTable } from "./RulesTable";
import CreateModal from "./CreateModal";
import { observer } from "mobx-react-lite";

interface Props {
    ruleProject: RuleProject;
}

export default observer(function RPRulesTab({ ruleProject }: Props) {
    const [modalOpen, setModalOpen] = useState(false);
    const [ruleType, setRuleType] = useState('');
    const standardRules = ruleProject.standardRules.map((rule) => ({ ...rule, type: "Standard Rule" }));
    const decisionTables = ruleProject.decisionTables.map((table) => ({ ...table, type: "Decision Table" }));
    const allRules = [...standardRules, ...decisionTables];


    const handleOpenModal = (type: string) => {
        setRuleType(type);
        setModalOpen(true);
    };

    const handleCloseModal = () => {
        setModalOpen(false);
    };

    return (
        <Grid spacing={3}>
            <Grid.Column width={16}>
                <Dropdown text='New' icon={'plus'} item>
                    <Dropdown.Menu>
                        <Dropdown.Item text='Decision Table' onClick={() => handleOpenModal('Decision Table')} />
                        <Dropdown.Item text='Standard Rule' onClick={() => handleOpenModal('Standard Rule')} />
                    </Dropdown.Menu>
                </Dropdown>
                <CreateModal ruleProjectId={ruleProject.id} open={modalOpen} onClose={handleCloseModal} type={ruleType} />
            </Grid.Column>
            <Grid.Column width={16}>
                <RulesTable data={allRules} />
            </Grid.Column>
        </Grid>
    );
})