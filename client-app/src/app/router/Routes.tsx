import { Navigate, RouteObject, createBrowserRouter } from "react-router-dom";
import App from "../layout/App";
import NotFound from "../../features/errors/NotFound";
import ServerError from "../../features/errors/ServerError";
import ProfilePage from "../../features/profiles/ProfilePage";
import RequireAuth from "./RequireAuth";
import Privacy from "../../features/subsctiptions/Privacy";
import SubscriptionPage from "../../features/subsctiptions/SubscriptionPage";
import RuleDashboard from "../../features/rules/dashboard/RuleDashboard";
import RuleDetails from "../../features/rules/details/RuleDetails";
import RuleForm from "../../features/rules/form/RuleForm";
import RuleProjectDetails from "../../features/ruleProjects/details/RuleProjectDetails";
import DTDetails from "../../features/decisionTables/DTDetails";
import OrganizationManagement from "../../features/organizations/OrganizationManagement";

export const routes: RouteObject[] = [
    {
        path: '/',
        element: <App />,
        children: [
            {
                element: <RequireAuth />, children: [
                    { path: 'profiles/:username', element: <ProfilePage /> },
                    { path: 'ruleprojects', element: <RuleDashboard /> },
                    { path: 'rules/:id', element: <RuleDetails /> },
                    { path: 'tables/:id', element: <DTDetails /> },
                    { path: 'ruleprojects/:id', element: <RuleProjectDetails /> },
                    { path: 'createRule', element: <RuleForm key='create' /> },
                    { path: 'organization', element: <OrganizationManagement /> }
                ]
            },
            { path: 'not-found', element: <NotFound /> },
            { path: '*', element: <Navigate replace to='/not-found' /> },
            { path: 'server-error', element: <ServerError /> }
        ]
    },
    {
        path: '/subscription',
        element: <SubscriptionPage />
    },
    {
        path: '/privacy',
        element: <Privacy />
    }
]

export const router = createBrowserRouter(routes);