import { useEffect, useState } from "react";
import { Button, Header, Segment } from "semantic-ui-react";
import { useStore } from "../../../app/stores/store";
import { observer } from "mobx-react-lite";
import { Link, useNavigate, useParams } from "react-router-dom";
import LoadingComponent from "../../../app/layout/LoadingComponent";
import { v4 as uuid } from 'uuid';
import { Formik, Form } from "formik";
import * as Yup from 'yup';
import MyTextInput from "../../../app/common/form/MyTextInput";
import { RuleFormValues } from "../../../app/models/rule";

export default observer(function RuleForm() {
    const { ruleStore } = useStore();
    const { createRule, updateRule, loadRule, loadingInitial } = ruleStore;
    const { id } = useParams();
    const navigate = useNavigate();

    const [rule, setRule] = useState<RuleFormValues>(new RuleFormValues());

    const conditionSchema = Yup.object().shape({
        field: Yup.string().required('Condition field is required'),
        operator: Yup.string().required('Condition operator is required'),
        value: Yup.string().required('Condition value is required'),
    });

    const actionSchema = Yup.object().shape({
        name: Yup.string().required('Action name is required'),
    });

    const validationSchema = Yup.object().shape({
        name: Yup.string().required('The rule name is required'),
        conditions: Yup.array()
            .of(conditionSchema)
            .min(1, `At least 1 condition is required`),
        actions: Yup.array()
            .of(actionSchema)
            .min(1, `At least 1 action is required`),
    });

    useEffect(() => {
        if (id) loadRule(id).then(rule => setRule(new RuleFormValues(rule)));
    }, [id, loadRule]);

    function handleFormSubmit(rule: RuleFormValues) {
        if (!rule.id) {
            rule.id = uuid();
            createRule(rule).then(() => navigate(`/rules/${rule.id}`));
        } else {
            updateRule(rule).then(() => navigate(`/rules/${rule.id}`));
        }
    }

    if (loadingInitial) return <LoadingComponent content="Loading rule..." />

    return (
        <Segment clearing>
            <Header content='Rule Details' sub color='teal' />
            <Formik
                validationSchema={validationSchema}
                enableReinitialize
                initialValues={rule}
                onSubmit={values => handleFormSubmit(values)}>
                {({ handleSubmit, isValid, isSubmitting, dirty, values, setFieldValue }) => (
                    <Form className='ui form' onSubmit={handleSubmit} autoComplete='off'>
                        <MyTextInput name='name' placeholder='Name' />

                        {values.conditions.map((_condition, i) => (
                            <div key={i}>
                                <MyTextInput placeholder='Condition Field' name={`conditions[${i}].field`} />
                                <MyTextInput placeholder='Condition Operator' name={`conditions[${i}].operator`} />
                                <MyTextInput placeholder='Condition Value' name={`conditions[${i}].value`} />
                            </div>
                        ))}
                        <Button type='button' onClick={() => setFieldValue('conditions', [...values.conditions, { field: '', operator: '', value: '' }])}>Add Condition</Button>

                        {values.actions.map((_action, i) => (
                            <div key={i}>
                                <MyTextInput placeholder='Action Name' name={`actions[${i}].name`} />
                            </div>
                        ))}
                        <Button type='button' onClick={() => setFieldValue('actions', [...values.actions, { name: '' }])}>Add Action</Button>

                        <Button
                            disabled={isSubmitting || !dirty || !isValid}
                            loading={isSubmitting}
                            floated='right'
                            positive
                            type='submit'
                            content='Submit'
                        />
                        <Button as={Link} to={'/rules'} floated='right' type='button' content='Cancel' />
                    </Form>
                )}
            </Formik>
        </Segment>
    )
});