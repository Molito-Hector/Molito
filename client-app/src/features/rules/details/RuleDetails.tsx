import { useEffect } from "react";
import { Grid } from "semantic-ui-react";
import { useStore } from "../../../app/stores/store";
import LoadingComponent from "../../../app/layout/LoadingComponent";
import { observer } from "mobx-react-lite";
import { useParams } from "react-router-dom";
import RuleDetailedInfo from "./RuleDetailedInfo";
import RuleDetailedChat from "./RuleDetailedChat";
import RuleDetailedHeader from "./RuleDetailedHeader";

export default observer(function RuleDetails() {

    const { ruleStore } = useStore();
    const { selectedRule: rule, loadRule, loadingInitial, clearSelectedRule } = ruleStore;
    const { id } = useParams();

    useEffect(() => {
        if (id) loadRule(id);
        return () => clearSelectedRule();
    }, [id, loadRule, clearSelectedRule])

    if (loadingInitial || !rule) return <LoadingComponent />;

    return (
        <Grid>
            <Grid.Column width={16}>
                <RuleDetailedHeader />
                <RuleDetailedInfo rule={rule} />
                <RuleDetailedChat ruleId={rule.id} />
            </Grid.Column>
        </Grid>
    )
})