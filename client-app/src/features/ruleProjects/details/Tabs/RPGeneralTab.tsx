import { Grid, Form, Segment } from "semantic-ui-react";
import { RuleProject } from '../../../../app/models/ruleProject';
import { format } from "date-fns";
import { observer } from "mobx-react-lite";

interface Props {
    ruleProject: RuleProject;
}

export default observer(function GeneralTab({ ruleProject }: Props) {
    return (
        <Segment clearing raised>
            <Grid>
                <Grid.Column width={8}>
                    <Form>
                        <Form.Field key="name" width={16}>
                            <label>Name</label>
                            <input value={ruleProject.name} readOnly />
                        </Form.Field>
                        <Form.Field key="createdAt">
                            <label>Created At</label>
                            <input
                                value={format(ruleProject.createdAt, 'MM/dd/yyyy')}
                                readOnly
                            />
                        </Form.Field>
                    </Form>
                </Grid.Column>
                <Grid.Column width={8}>
                    <Form>
                        <Form.Field inline key="description">
                            <label>Description</label>
                            <Form.TextArea value={ruleProject.description} readOnly />
                        </Form.Field>
                    </Form>
                </Grid.Column>
            </Grid>
        </Segment>
    );
})