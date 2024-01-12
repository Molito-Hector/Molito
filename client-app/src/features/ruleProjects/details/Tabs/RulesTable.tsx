import { format } from "date-fns";
import { Rule } from "../../../../app/models/rule";
import { Card, Icon, Popup, Table } from "semantic-ui-react";
import { Link } from "react-router-dom";
import { useCallback, useMemo, useReducer, useState } from "react";
import DeleteModal from "./DeleteModal";

interface TableProps {
  data: Rule[];
}

export function RulesTable({ data }: TableProps) {

  const [ruleToDelete, setRuleToDelete] = useState<Rule | null>(null);
  const [deleteModalOpen, setDeleteModalOpen] = useState(false);

  const handleOpenDeleteModal = (rule: Rule) => {
    setRuleToDelete(rule);
    setDeleteModalOpen(true);
  };

  const handleCloseDeleteModal = () => {
    setDeleteModalOpen(false);
  };

  type State = {
    sortColumn: string;
    sortDirection: "ascending" | "descending";
  };

  const initialState: State = {
    sortColumn: "name",
    sortDirection: "ascending",
  };

  const [state, dispatch] = useReducer(reducer, initialState);

  const handleSort = useCallback(
    (column: string) => {
      if (state.sortColumn === column) {
        dispatch({ type: "TOGGLE_SORT_DIRECTION" });
      } else {
        dispatch({ type: "SET_SORT_COLUMN", payload: column });
        dispatch({ type: "SET_SORT_DIRECTION", payload: "ascending" });
      }
    },
    [state.sortColumn, dispatch]
  );

  type Action =
    | { type: "SET_SORT_COLUMN"; payload: string }
    | { type: "SET_SORT_DIRECTION"; payload: "ascending" | "descending" }
    | { type: "TOGGLE_SORT_DIRECTION" };



  function reducer(state: State, action: Action): State {
    switch (action.type) {
      case "SET_SORT_COLUMN":
        return { ...state, sortColumn: action.payload };
      case "SET_SORT_DIRECTION":
        return { ...state, sortDirection: action.payload };
      case "TOGGLE_SORT_DIRECTION":
        return {
          ...state,
          sortDirection: state.sortDirection === "ascending" ? "descending" : "ascending",
        };
      default:
        throw new Error("Invalid action type");
    }
  }

  const sortedRules = useMemo(() => {
    return [...data].sort((a, b) => {
      const isAscending = state.sortDirection === "ascending" ? 1 : -1;

      if (!(state.sortColumn in a) || !(state.sortColumn in b)) {
        throw new Error("Invalid sort column");
      }

      const sortA = a[state.sortColumn];
      const sortB = b[state.sortColumn];

      if (sortA! < sortB!) {
        return -1 * isAscending;
      } else if (sortA! > sortB!) {
        return 1 * isAscending;
      } else {
        return 0;
      }
    });
  }, [data, state.sortColumn, state.sortDirection]);

  return (
    <>
      <Table sortable celled fixed striped color='purple'>
        <Table.Header>
          <Table.Row>
            <Table.HeaderCell sorted={state.sortColumn === "name" ? state.sortDirection : undefined} onClick={() => handleSort("name")}>
              Name
            </Table.HeaderCell>
            <Table.HeaderCell sorted={state.sortColumn === "description" ? state.sortDirection : undefined} onClick={() => handleSort("description")}>
              Description
            </Table.HeaderCell>
            <Table.HeaderCell sorted={state.sortColumn === "createdAt" ? state.sortDirection : undefined} onClick={() => handleSort("createdAt")}>
              Created At
            </Table.HeaderCell>
            <Table.HeaderCell sorted={state.sortColumn === "type" ? state.sortDirection : undefined} onClick={() => handleSort("type")}>
              Type
            </Table.HeaderCell>
            <Table.HeaderCell>Actions</Table.HeaderCell>
          </Table.Row>
        </Table.Header>
        <Table.Body>
          {sortedRules.map((rule) => (
            <Table.Row key={rule.id}>
              <Table.Cell>
                <Link to={rule.type === 'Decision Table' ? `/tables/${rule.id}` : `/rules/${rule.id}`}>
                  {rule.name}
                </Link>
              </Table.Cell>
              <Table.Cell>{rule.description}</Table.Cell>
              <Table.Cell>{format(rule.createdAt, 'MM/dd/yyyy')}</Table.Cell>
              <Table.Cell>{rule.type}</Table.Cell>
              <Table.Cell>
                <Popup
                  hoverable
                  trigger={
                    <Icon name="exchange"
                      style={{ opacity: 0.3, transition: 'opacity 0.2s' }}
                      className="deletable-column"
                    />
                  }
                >
                  <Popup.Content>
                    <Card>
                      <Card.Content>
                        <Card.Header>{rule.name}</Card.Header>
                        <Card.Description>
                          To call this rule, use the following URL: /api/ruleengine/{rule.id}/executeTable
                        </Card.Description>
                      </Card.Content>
                    </Card>
                  </Popup.Content>
                </Popup>
                <Icon
                  name="trash"
                  onClick={() => handleOpenDeleteModal(rule)}
                  style={{ opacity: 0.3, transition: 'opacity 0.2s', cursor: 'pointer' }}
                  className="deletable-column"
                />
              </Table.Cell>
            </Table.Row>
          ))}
        </Table.Body>
      </Table>
      <DeleteModal open={deleteModalOpen} onClose={handleCloseDeleteModal} ruleId={ruleToDelete?.id || ''} ruleName={ruleToDelete?.name || ''} type={ruleToDelete?.type || "decisionTable"} />
    </>
  );
}