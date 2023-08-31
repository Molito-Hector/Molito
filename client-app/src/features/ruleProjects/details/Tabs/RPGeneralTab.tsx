import { Grid, Form } from "semantic-ui-react";
import { RuleProject } from '../../../../app/models/ruleProject';
import { format } from "date-fns";
import { observer } from "mobx-react-lite";

interface Props {
    ruleProject: RuleProject;
}

export default observer(function GeneralTab({ ruleProject }: Props) {
    return (
        <Grid>
            <Grid.Column width={8}>
                <Form>
                    <Form.Field width={16}>
                        <label>Name</label>
                        <input value={ruleProject.name} readOnly />
                    </Form.Field>
                    <Form.Field>
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
                    <Form.Field inline>
                        <label>Description</label>
                        <Form.TextArea value={ruleProject.description} readOnly />
                    </Form.Field>
                </Form>
            </Grid.Column>
        </Grid>
    );
})