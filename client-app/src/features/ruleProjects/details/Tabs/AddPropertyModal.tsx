import React from "react";
import { Modal, Form, Button } from "semantic-ui-react";
import { Formik, FieldArray, ArrayHelpers, FormikTouched, FormikErrors } from "formik";
import * as Yup from "yup";
import { RuleProperty } from "../../../../app/models/ruleProject";

interface Props {
  open: boolean;
  onClose: () => void;
  onSubmit: (property: RuleProperty) => void;
}

const PROPERTY_TYPES = [
  { key: 'StringType', text: 'String', value: 'StringType' },
  { key: 'NumberType', text: 'Number', value: 'NumberType' },
  { key: 'BooleanType', text: 'Boolean', value: 'BooleanType' },
  { key: 'ObjectType', text: 'Object', value: 'ObjectType' }
];

const DIRECTIONS = [
  { key: 'I', text: 'Input', value: 'I' },
  { key: 'O', text: 'Output', value: 'O' },
  { key: 'B', text: 'Bidirectional', value: 'B' }
];

const AddPropertyModal: React.FC<Props> = ({ open, onClose, onSubmit }) => {
  const initialValues: RuleProperty = {
    id: "",
    name: "",
    type: "StringType",
    direction: "",
    subProperties: [],
  };

  const validationSchema = Yup.object({
    name: Yup.string().required("Name is required"),
    type: Yup.string().required("Type is required"),
    direction: Yup.string().required("Direction is required"),
    subProperties: Yup.array().of(
      Yup.object({
        name: Yup.string().required("Subproperty name is required"),
        type: Yup.string().required("Subproperty type is required"),
      })
    ),
  });

  return (
    <Modal open={open} onClose={onClose}>
      <Modal.Header>Add Property</Modal.Header>
      <Modal.Content>
        <Formik
          initialValues={initialValues}
          validationSchema={validationSchema}
          onSubmit={(values) => onSubmit(values)}
        >
          {({
            values,
            errors,
            touched,
            handleChange,
            handleBlur,
            handleSubmit,
            isSubmitting,
            setFieldValue,
          }) => (
            <Form onSubmit={handleSubmit}>
              <Form.Input
                label="Name"
                name="name"
                value={values.name}
                onChange={handleChange}
                onBlur={handleBlur}
                error={touched.name && errors.name}
              />
              <Form.Dropdown
                label="Type"
                name="type"
                fluid
                selection
                options={PROPERTY_TYPES}
                value={values.type}
                onChange={(_, { value }) => setFieldValue('type', value)}
                error={touched.type && errors.type}
              />
              <Form.Dropdown
                label="Direction"
                name="direction"
                fluid
                selection
                options={DIRECTIONS}
                value={values.direction}
                onChange={(_, { value }) => setFieldValue('direction', value)}
                error={touched.type && errors.type}
              />

              <FieldArray
                name="subProperties"
                render={(arrayHelpers: ArrayHelpers) => (
                  <div>
                    {values.subProperties && values.subProperties.length > 0 ? (
                      values.subProperties.map((subProperty, index) => (
                        <div key={index}>
                          <Form.Input
                            label="Subproperty Name"
                            name={`subProperties.${index}.name`}
                            value={subProperty.name}
                            onChange={handleChange}
                            onBlur={handleBlur}
                            error={(touched.subProperties as FormikTouched<RuleProperty>[] | undefined)?.[index]?.name && (errors.subProperties as FormikErrors<RuleProperty>[] | undefined)?.[index]?.name}
                          />
                          <Form.Dropdown
                            label="Subproperty Type"
                            name={`subProperties.${index}.type`}
                            fluid
                            selection
                            options={PROPERTY_TYPES}
                            value={subProperty.type}
                            onChange={(_, { value }) => setFieldValue(`subProperties.${index}.type`, value)}
                            error={(touched.subProperties as FormikTouched<RuleProperty>[] | undefined)?.[index]?.type && (errors.subProperties as FormikErrors<RuleProperty>[] | undefined)?.[index]?.type}
                          />
                          <Button
                            type="button"
                            onClick={() => arrayHelpers.remove(index)}
                          >
                            Remove
                          </Button>
                        </div>
                      ))
                    ) : null}
                    <Button
                      type="button"
                      onClick={() => arrayHelpers.push({ name: "", type: "" })}
                    >
                      Add Subproperty
                    </Button>
                  </div>
                )}
              />

              <Button loading={isSubmitting} type="submit" disabled={isSubmitting}>
                Add
              </Button>
            </Form>
          )}
        </Formik>
      </Modal.Content>
    </Modal>
  );
};

export default AddPropertyModal;