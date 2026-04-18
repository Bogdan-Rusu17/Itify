import { useAppSelector } from "@application/store"
import {useGetUser} from "@infrastructure/apis/api-management";
import { UserRoleEnum } from "@infrastructure/apis/client";
import { isUndefined } from "lodash";

/**
 * You can use this hook to retrieve the own user from the backend.
 * You can create new hooks by using and combining other hooks.
 */
export const useOwnUser = () => {
    const { userId } = useAppSelector(x => x.profileReducer); // Get the own user id from the redux storage.
    const { data } = useGetUser(userId); // Get the client for the API.
    return data?.response;
}

/**
 * This hook returns if the current user has the given role.
 */
export const useOwnUserHasRole = (role: UserRoleEnum) => {
    const ownUser = useOwnUser();

    if (isUndefined(ownUser)) {
        return;
    }

    return ownUser?.role === role;
}

/**
 * This hook returns if the JWT token has expired or not.
 */
export const useTokenHasExpired = () => {
    const { loggedIn, exp } = useAppSelector(x => x.profileReducer);
    const now = Date.now() / 1000;

    return {
        loggedIn,
        hasExpired: !exp || exp < now
    };
}