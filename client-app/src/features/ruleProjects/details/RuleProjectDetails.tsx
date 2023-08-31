import { useEffect, useState } from "react";
import { Segment, Tab } from "semantic-ui-react";
import { useStore } from "../../../app/stores/store";
import LoadingComponent from "../../../app/layout/LoadingComponent";
import { observer } from "mobx-react-lite";
import { useParams } from "react-router-dom";
import RPGeneralTab from "./Tabs/RPGeneralTab";
import RPDataModelTab from "./Tabs/RPDataModelTab";
import RPRulesTab from "./Tabs/RPRulesTab";

export default observer(function RuleProjectDetails() {

    const { ruleProjectStore } = useStore();
    const { selectedRuleProject: ruleProject, loadRuleProject, loadingInitial, clearSelectedRuleProject } = ruleProjectStore;
    const { id } = useParams();
    const [activeTab, setActiveTab] = useState(0);

    useEffect(() => {
        if (id) loadRuleProject(id);
        return () => clearSelectedRuleProject();
    }, [id, loadRuleProject, clearSelectedRuleProject])

    if (loadingInitial || !ruleProject) return <LoadingComponent />;

    const panes = [
        {
            menuItem: "General",
            render: () => <RPGeneralTab ruleProject={ruleProject} />,
        },
        {
            menuItem: "Data Model",
            render: () => <RPDataModelTab ruleProject={ruleProject} />,
        },
        {
            menuItem: "Rules",
            render: () => <RPRulesTab ruleProject={ruleProject} />,
        }
    ];

    return (
        <Segment clearing raised>
            <Tab
                menu={{ secondary: true, pointing: true }}
                panes={panes}
                activeIndex={activeTab}
                onTabChange={(_, data) => setActiveTab(data.activeIndex as number)}
            />
        </Segment>
    )
})