import React from "react";
import { Modal, Form, Button } from "semantic-ui-react";
import { Formik, FieldArray, ArrayHelpers } from "formik";
import * as Yup from "yup";
import { RuleProperty } from "../../../../app/models/ruleProject";

interface Props {
  open: boolean;
  onClose: () => void;
  onSubmit: (property: RuleProperty) => void;
}

const AddPropertyModal: React.FC<Props> = ({ open, onClose, onSubmit }) => {
  const initialValues: RuleProperty = {
    id: "",
    name: "",
    type: "",
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
              <Form.Input
                label="Type"
                name="type"
                value={values.type}
                onChange={handleChange}
                onBlur={handleBlur}
                error={touched.type && errors.type}
              />
              <Form.Input
                label="Direction"
                name="direction"
                value={values.direction}
                onChange={handleChange}
                onBlur={handleBlur}
                error={touched.direction && errors.direction}
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
                            error={(touched.subProperties as any)?.[index]?.name && (errors.subProperties as any)?.[index]?.name}
                          />
                          <Form.Input
                            label="Subproperty Type"
                            name={`subProperties.${index}.type`}
                            value={subProperty.type}
                            onChange={handleChange}
                            onBlur={handleBlur}
                            error={(touched.subProperties as any)?.[index]?.type && (errors.subProperties as any)?.[index]?.type}
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