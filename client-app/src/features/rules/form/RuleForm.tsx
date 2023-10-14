import { observer } from "mobx-react-lite";

export default observer(function RuleForm() {

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