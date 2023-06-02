import { Link } from "react-router-dom";
import { Container, Header, Segment, Image, Button } from "semantic-ui-react";
import { observer } from "mobx-react-lite";

export default observer(function SubscriptionPage() {
    return (
        <Segment inverted textAlign="center" vertical className='masthead'>
            <Container text>
                <Header as='h1' inverted>
                    <Image size='massive' src='/assets/logo.png' alt='logo' style={{ marginBottom: 12 }} />
                    Molito
                </Header>
                <>
                    <Header as='h2' inverted content='Thank you for subscribing to Molito AIDE' />
                    <Button as={Link} to='/' size='huge' inverted>
                        Go to the Home Page!
                    </Button>
                </>
            </Container>
        </Segment>
    )
})