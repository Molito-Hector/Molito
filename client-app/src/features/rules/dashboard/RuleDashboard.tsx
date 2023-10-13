import { useEffect, useState } from "react";
import { Grid, Segment } from "semantic-ui-react";
import { useStore } from "../../../app/stores/store";
import { observer } from "mobx-react-lite";
import { PagingParams } from "../../../app/models/pagination";
import InfiniteScroll from "react-infinite-scroller";
import RuleListItemPlaceholder from "./RuleListItemPlaceholder";
import RuleList from "./RuleList";

export default observer(function RuleDashboard() {

    const { ruleProjectStore } = useStore();
    const { loadRuleProjects, ruleProjectRegistry, setPagingParams, pagination } = ruleProjectStore;
    const [loadingNext, setLoadingNext] = useState(false);

    function handleGetNext() {
        setLoadingNext(true);
        setPagingParams(new PagingParams(pagination!.currentPage + 1))
        loadRuleProjects().then(() => setLoadingNext(false));
    }

    useEffect(() => {
        if (ruleProjectRegistry.size <= 1) loadRuleProjects();
    }, [loadRuleProjects, ruleProjectRegistry.size])

    return (
        <Segment clearing raised>
            <Grid>
                <Grid.Column width='16'>
                    {ruleProjectStore.loadingInitial && !loadingNext ? (
                        <>
                            <RuleListItemPlaceholder />
                            <RuleListItemPlaceholder />
                        </>
                    ) : (
                        <InfiniteScroll
                            pageStart={0}
                            loadMore={handleGetNext}
                            hasMore={!loadingNext && !!pagination && pagination.currentPage < pagination.totalPages}
                            initialLoad={false}
                        >
                            <RuleList />
                        </InfiniteScroll>
                    )}
                </Grid.Column>
            </Grid>
        </Segment>
    )
})