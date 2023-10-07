import axios, { AxiosError, AxiosResponse } from "axios";
import { Activity, ActivityFormValues } from "../models/activity";
import { toast } from "react-toastify";
import { router } from "../router/Routes";
import { store } from "../stores/store";
import { User, UserFormValues } from "../models/user";
import { Photo, Profile, UserActivity } from "../models/profile";
import { PaginatedResult } from "../models/pagination";
import { Action, Condition, Rule, RuleFormValues } from "../models/rule";
import { RPFormValues, RuleProject, RuleProperty } from "../models/ruleProject";
import { DTFormValues, DecisionTable } from "../models/decisionTable";
import { OrgFormValues, Organization } from "../models/organization";

axios.defaults.baseURL = process.env.REACT_APP_API_URL;

axios.interceptors.response.use(async response => {
    const pagination = response.headers['pagination'];
    if (pagination) {
        response.data = new PaginatedResult(response.data, JSON.parse(pagination));
        return response as AxiosResponse<PaginatedResult<any>>;
    }
    return response;
}, (error: AxiosError) => {
    const { data, status, config } = error.response as AxiosResponse;
    switch (status) {
        case 400:
            if (!data.errors) toast.error(data);
            else if (config.method === "get" && data.errors.hasOwnProperty("id")) {
                router.navigate("/not-found");
                break;
            } else if (data.errors) {
                const modalStateErrors = [];
                for (const key in data.errors) {
                    if (data.errors[key]) {
                        modalStateErrors.push(data.errors[key]);
                    }
                }
                throw modalStateErrors.flat();
            }
            break;
        case 401:
            toast.error('Unauthorized');
            break;
        case 403:
            toast.error('Forbidden');
            break;
        case 404:
            router.navigate('/not-found');
            break;
        case 500:
            store.commonStore.setServerError(data);
            router.navigate('/server-error');
            break;
    }
    return Promise.reject(error);
})

const responseBody = <T>(response: AxiosResponse<T>) => response.data;

axios.interceptors.request.use(config => {
    const token = store.commonStore.token;
    if (token && config.headers) config.headers.Authorization = `Bearer ${token}`;
    return config;
})

const requests = {
    get: <T>(url: string) => axios.get<T>(url).then(responseBody),
    post: <T>(url: string, body: {}) => axios.post<T>(url, body).then(responseBody),
    put: <T>(url: string, body: {}) => axios.put<T>(url, body).then(responseBody),
    del: <T>(url: string) => axios.delete<T>(url).then(responseBody),
}

const Activities = {
    list: (params: URLSearchParams) => axios.get<PaginatedResult<Activity[]>>('/activities', { params }).then(responseBody),
    details: (id: string) => requests.get<Activity>(`/activities/${id}`),
    create: (activity: ActivityFormValues) => requests.post<void>('/activities', activity),
    update: (activity: ActivityFormValues) => requests.put<void>(`/activities/${activity.id}`, activity),
    delete: (id: string) => requests.del<void>(`/activities/${id}`),
    attend: (id: string) => requests.post<void>(`/activities/${id}/attend`, {})
}

const Organizations = {
    create: (organization: OrgFormValues) => requests.post<void>('/organizations', organization),
    details: (id: string) => requests.get<Organization>(`/organizations/${id}`),
    updateMember: (id: string, username: string) => requests.post<void>(`/organizations/${id}/updateMember`, { username })
}

const Rules = {
    list: (params: URLSearchParams) => axios.get<PaginatedResult<Rule[]>>('/rules', { params }).then(responseBody),
    details: (id: string) => requests.get<Rule>(`/rules/${id}`),
    create: (rule: RuleFormValues) => requests.post<void>('/rules', rule),
    update: (rule: RuleFormValues) => requests.put<void>(`/rules/${rule.id}`, rule),
    delete: (id: string) => requests.del<void>(`/rules/${id}`)
}

const RuleProjects = {
    list: (params: URLSearchParams) => axios.get<PaginatedResult<RuleProject[]>>('ruleprojects', { params }).then(responseBody),
    details: (id: string) => requests.get<RuleProject>(`/ruleprojects/${id}`),
    create: (ruleProject: RPFormValues) => requests.post<void>('ruleprojects', ruleProject),
    delete: (id: string) => requests.del<void>(`/ruleprojects/${id}`),
    addProperties: (id: string, properties: RuleProperty[]) => requests.post<void>(`/ruleprojects/${id}/addProperties`, properties),
    removeProperty: (id: string, propId: string) => requests.del<void>(`/ruleprojects/${id}/removeProperty/${propId}`),
    updateMember: (id: string, username: string) => requests.post<void>(`/ruleprojects/${id}/updateMember`, { username })
}

const DecisionTables = {
    details: (id: string) => requests.get<DecisionTable>(`/tables/${id}`),
    create: (table: DTFormValues) => requests.post<void>('/tables', table),
    populate: (id: string, table: DecisionTable) => requests.post<void>(`/tables/${id}/populate`, table),
    addColumn: (id: string, condition: Condition, predicate: string) => requests.post<void>(`/tables/${id}/addColumn?predicate=${predicate}`, condition),
    addActionColumn: (id: string, action: Action, predicate: string) => requests.post<void>(`/tables/${id}/addActionColumn?predicate=${predicate}`, action),
    editActionColumn: (id: string, action: Action) => requests.put<void>(`/tables/${id}/actions/column`, action),
    delete: (id: string) => requests.del<void>(`/tables/${id}`),
    deleteColumn: (id: string) => requests.del<void>(`/tables/${id}/removeColumn`),
    deleteActionColumn: (id: string) => requests.del<void>(`/tables/${id}/removeActionColumn`)
}

const Account = {
    current: () => requests.get<User>('/account'),
    getUser: (username: string) => axios.get<User>('/account/getUser', { params: { Username: username } }),
    login: (user: UserFormValues) => requests.post<User>('/account/login', user),
    register: (user: UserFormValues) => requests.post<User>('/account/register', user)
}

const Profiles = {
    get: (username: string) => requests.get<Profile>(`/profiles/${username}`),
    uploadPhoto: (file: Blob) => {
        let formData = new FormData();
        formData.append('File', file);
        return axios.post<Photo>('photos', formData, {
            headers: { 'Content-Type': 'multipart/form-data' }
        })
    },
    setMainPhoto: (id: string) => requests.post(`/photos/${id}/setMain`, {}),
    deletePhoto: (id: string) => requests.del(`/photos/${id}`),
    updateProfile: (profile: Partial<Profile>) => requests.put(`/profiles`, profile),
    updateFollowing: (username: string) => requests.post(`/follow/${username}`, {}),
    listFollowings: (username: string, predicate: string) => requests.get<Profile[]>(`/follow/${username}?predicate=${predicate}`),
    listActivities: (username: string, predicate: string) => requests.get<UserActivity[]>(`/profiles/${username}/activities?predicate=${predicate}`)
}

const agent = {
    Activities,
    Account,
    Profiles,
    Rules,
    RuleProjects,
    DecisionTables,
    Organizations
}

export default agent;