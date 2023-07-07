import { Navigate, ScrollRestoration } from "react-router-dom";
import { Container, Header, Segment, Image, Button } from "semantic-ui-react";
import { observer } from "mobx-react-lite";
import { useStore } from "../../app/stores/store";
import { useEffect } from "react";
import LoadingComponent from "../../app/layout/LoadingComponent";
import ModalContainer from "../../app/common/modals/ModalContainer";
import { ToastContainer } from "react-toastify";
import RegisterForm from "../users/RegisterForm";

export default observer(function SubscriptionPage() {
    const { subscriptionStore, modalStore } = useStore();
    const { subFound, subscription } = subscriptionStore;

    useEffect(() => {
        const urlParams = new URLSearchParams(window.location.search);
        const encodedURL = urlParams.get('_sub');

        if (encodedURL) {
            subscriptionStore.getSubscriptionData(encodedURL);
        }
    }, [subscriptionStore]);

    if (subscriptionStore.loading) return <LoadingComponent content='Loading subscription...' />

    if (!subFound) {
        return <Navigate to='/' />
    }

    return (
        <>
            <ScrollRestoration />
            <ModalContainer />
            <ToastContainer position='bottom-right' hideProgressBar theme='colored' />
            <Segment inverted textAlign="center" vertical className='masthead'>
                <Container text>
                    <Header as='h1' inverted>
                        <Image size='massive' src='/assets/logo.png' alt='logo' style={{ marginBottom: 12 }} />
                        Molito
                    </Header>
                    <Header as='h2' inverted>
                        {subscription?.purchaser.userEmail}
                    </Header>
                    <>
                        <Header as='h2' inverted content='Thank you for subscribing to Molito AIDE Preview' />
                        <Button onClick={() => modalStore.openModal(<RegisterForm />)} size='huge' inverted>
                            Complete registration
                        </Button>
                    </>
                </Container>
            </Segment>
        </>
    )
})