import MyTextInput from "../../../app/common/form/MyTextInput";

interface Props {
    name: string;
}

export const ConditionForm: React.FC<Props> = ({ name }) => {
    return (
        <>
            <MyTextInput placeholder="Condition Field" name={`${name}.field`} />
            <MyTextInput placeholder="Condition Operator" name={`${name}.operator`} />
            <MyTextInput placeholder="Condition Value" name={`${name}.value`} />
            <MyTextInput placeholder="Logical Operator" name={`${name}.logicalOperator`} />
        </>
    );
};