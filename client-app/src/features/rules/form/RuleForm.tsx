import { useEffect, useState } from "react";
import { Accordion, Button, FormField, Header, Segment } from "semantic-ui-react";
import { useStore } from "../../../app/stores/store";
import { observer } from "mobx-react-lite";
import { Link, useNavigate, useParams } from "react-router-dom";
import LoadingComponent from "../../../app/layout/LoadingComponent";
import { v4 as uuid } from 'uuid';
import { Formik, Form } from "formik";
import * as Yup from 'yup';
import MyTextInput from "../../../app/common/form/MyTextInput";
import { RuleFormValues } from "../../../app/models/rule";
import { ConditionForm } from "./ConditionForm";

export default observer(function RuleForm() {
    const { ruleStore } = useStore();
    // const { createRule, updateRule, loadRule, loadingInitial } = ruleStore;
    const { id } = useParams();
    const navigate = useNavigate();

    const propertyTypes = [
        { key: 'type1', text: 'Type 1', value: 'Type1' },
        { key: 'type2', text: 'Type 2', value: 'Type2' },
        // ...other property types...
    ];

    const [rule, setRule] = useState<RuleFormValues>(new RuleFormValues());

    const subConditionSchema = Yup.object().shape({
        field: Yup.string().required('Condition field is required'),
        operator: Yup.string().required('Condition operator is required'),
        value: Yup.string().required('Condition value is required'),
        logicalOperator: Yup.string(),
    });

    const conditionSchema = Yup.object().shape({
        field: Yup.string().required('Condition field is required'),
        operator: Yup.string().required('Condition operator is required'),
        value: Yup.string().required('Condition value is required'),
        logicalOperator: Yup.string().required(),
        subConditions: Yup.array().of(subConditionSchema),
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

    // useEffect(() => {
    //     if (id) loadRule(id).then(rule => setRule(new RuleFormValues(rule)));
    // }, [id, loadRule]);

    // function handleFormSubmit(rule: RuleFormValues) {
    //     if (!rule.id) {
    //         rule.id = uuid();
    //         createRule(rule).then(() => navigate(`/rules/${rule.id}`));
    //     } else {
    //         updateRule(rule).then(() => navigate(`/rules/${rule.id}`));
    //     }
    // }

    // if (loadingInitial) return <LoadingComponent content="Loading rule..." />

    return (
        <></>
        // <Segment clearing>
        //     <Header content='Rule Details' sub color='teal' />
        //     <Formik
        //         validationSchema={validationSchema}
        //         enableReinitialize
        //         initialValues={rule}
        //         onSubmit={values => handleFormSubmit(values)}>
        //         {({ handleSubmit, isValid, isSubmitting, dirty, values, setFieldValue }) => (
        //             <Form className='ui form' onSubmit={handleSubmit} autoComplete='off'>
        //                 <FormField>
        //                     <label>Rule Name</label>
        //                     <MyTextInput name='name' placeholder='Name' />
        //                 </FormField>

        //                 <Accordion styled fluid>
        //                     {values.conditions.map((_condition, i) => (
        //                         <div key={i}>
        //                             <Accordion.Title>{`Condition ${i + 1}`}</Accordion.Title>
        //                             <Accordion.Content>
        //                                 <ConditionForm name={`conditions[${i}]`} />
        //                                 {_condition.subConditions?.map((_subCondition, j) => (
        //                                     <div key={j}>
        //                                         <ConditionForm name={`conditions[${i}].subConditions[${j}]`} />
        //                                     </div>
        //                                 ))}
        //                                 <Button
        //                                     type="button"
        //                                     onClick={() =>
        //                                         setFieldValue(`conditions[${i}].subConditions`, [
        //                                             ...values.conditions[i].subConditions!,
        //                                             { field: "", operator: "", value: "", logicalOperator: "" },
        //                                         ])
        //                                     }
        //                                 >
        //                                     Add Sub-Condition
        //                                 </Button>
        //                             </Accordion.Content>
        //                         </div>
        //                     ))}
        //                 </Accordion>
        //                 <Button
        //                     type='button'
        //                     onClick={() => setFieldValue('conditions', [...values.conditions, { field: '', operator: '', value: '', logicalOperator: '', subConditions: [] }])}>
        //                     Add Condition
        //                 </Button>

        //                 {/* {values.actions.map((_action, i) => (
        //                     <div key={i}>
        //                         <MyTextInput placeholder='Action Name' name={`actions[${i}].name`} />
        //                     </div>
        //                 ))} */}
        //                 {/* <Button type='button' onClick={() => setFieldValue('actions', [...values.actions, { name: '' }])}>Add Action</Button> */}

        //                 <Button
        //                     disabled={isSubmitting || !dirty || !isValid}
        //                     loading={isSubmitting}
        //                     floated='right'
        //                     positive
        //                     type='submit'
        //                     content='Submit'
        //                 />
        //                 <Button as={Link} to={'/rules'} floated='right' type='button' content='Cancel' />
        //             </Form>
        //         )}
        //     </Formik>
        // </Segment>
    )
});