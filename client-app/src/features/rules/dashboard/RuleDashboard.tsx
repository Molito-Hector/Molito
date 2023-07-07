import { useEffect, useState } from "react";
import { Grid, Loader } from "semantic-ui-react";
import { useStore } from "../../../app/stores/store";
import { observer } from "mobx-react-lite";
import ActivityFilters from "./ActivityFilters";
import { PagingParams } from "../../../app/models/pagination";
import InfiniteScroll from "react-infinite-scroller";
import RuleListItemPlaceholder from "./RuleListItemPlaceholder";
import RuleList from "./RuleList";

export default observer(function RuleDashboard() {

    const { ruleStore } = useStore();
    const { loadRules, ruleRegistry, setPagingParams, pagination } = ruleStore;
    const [loadingNext, setLoadingNext] = useState(false);

    function handleGetNext() {
        setLoadingNext(true);
        setPagingParams(new PagingParams(pagination!.currentPage + 1))
        loadRules().then(() => setLoadingNext(false));
    }

    useEffect(() => {
        if (ruleRegistry.size <= 1) loadRules();
    }, [loadRules, ruleRegistry.size])

    return (
        <Grid>
            <Grid.Column width='10'>
                {ruleStore.loadingInitial && !loadingNext ? (
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
            <Grid.Column width='6'>
                <ActivityFilters />
            </Grid.Column>
            <Grid.Column width={10}>
                <Loader active={loadingNext} />
            </Grid.Column>
        </Grid>
    )
})