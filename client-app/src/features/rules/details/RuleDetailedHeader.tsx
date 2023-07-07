import { observer } from 'mobx-react-lite';
import { Header, Item, Segment, Image } from 'semantic-ui-react'
import { Link } from 'react-router-dom';
import { Rule } from '../../../app/models/rule';

const ruleImageStyle = {
    filter: 'brightness(30%)',
};

const ruleImageTextStyle = {
    position: 'absolute',
    bottom: '5%',
    left: '5%',
    width: '100%',
    height: 'auto',
    color: 'white'
};

interface Props {
    rule: Rule
}

export default observer(function RuleDetailedHeader({ rule }: Props) {
    return (
        <Segment.Group>
            <Segment basic attached='top' style={{ padding: '0' }}>
                <Image src={`/assets/categoryImages/film.jpg`} fluid style={ruleImageStyle} />
                <Segment style={ruleImageTextStyle} basic>
                    <Item.Group>
                        <Item>
                            <Item.Content>
                                <Header
                                    size='huge'
                                    content={rule.name}
                                    style={{ color: 'white' }}
                                />
                                <p>
                                    Rule ID: <strong><Link to={`/rules/${rule.id}`}>{rule.id}</Link></strong>
                                </p>
                            </Item.Content>
                        </Item>
                    </Item.Group>
                </Segment>
            </Segment>
        </Segment.Group>
    )
})
