import {useAppSelector} from "@application/store";
import {Configuration, UserAddRecord, UserApi} from "../client";
import {useMutation, useQuery, useQueryClient} from "@tanstack/react-query";
import {isEmpty} from "lodash";

/**
 * Use constants to identify mutations and queries.
 */
const getUsersQueryKey = "getUsersQuery";
const getUserQueryKey = "getUserQuery";
const addUserMutationKey = "addUserMutation";
const deleteUserMutationKey = "deleteUserMutation";

const getFactory = (token: string | null) => new UserApi(new Configuration({accessToken: token ?? ""}));

export const useGetUsers = (page: number, pageSize: number, search: string = "") => {
    const {token} = useAppSelector(x => x.profileReducer); // You can use the data form the Redux storage.

    return {
        ...useQuery({
            queryKey: [getUsersQueryKey, token, page, pageSize, search],
            queryFn: async () => await getFactory(token).apiUserGetPageGet({page, pageSize, search}),
            refetchInterval: Infinity, // User information may not be frequently updated so refetching the data periodically is not necessary.
            refetchOnWindowFocus: false // This disables fetching the user information from the backend when focusing on the current window.
        }),
        queryKey: getUsersQueryKey
    };
}

export const useGetUser = (id: string | null) => {
    const {token} = useAppSelector(x => x.profileReducer); // You can use the data form the Redux storage.

    return {
        ...useQuery({
            queryKey: [getUserQueryKey, token, id],
            queryFn: async () => await getFactory(token).apiUserGetByIdIdGet({id: id ?? ""}),
            refetchInterval: Infinity, // User information may not be frequently updated so refetching the data periodically is not necessary.
            refetchOnWindowFocus: false, // This disables fetching the user information from the backend when focusing on the current window.
            enabled: !isEmpty(id)
        }),
        queryKey: getUserQueryKey
    };
}

export const useAddUser = () => {
    const {token} = useAppSelector(x => x.profileReducer); // You can use the data form the Redux storage.
    const queryClient = useQueryClient();

    return useMutation({
        mutationKey: [addUserMutationKey, token],
        mutationFn: async (userAddRecord: UserAddRecord) => {
            const result = await getFactory(token).apiUserAddPost({userAddRecord});
            await queryClient.invalidateQueries({queryKey: [getUsersQueryKey], type: "all"});  // If the form submission succeeds then some other queries need to be refresh so invalidate them to do a refresh.
            await queryClient.invalidateQueries({queryKey: [getUserQueryKey], type: "all"});

            return result;
        }
    })
}

export const useDeleteUser = () => {
    const {token} = useAppSelector(x => x.profileReducer); // You can use the data form the Redux storage.
    const queryClient = useQueryClient();

    return useMutation({
        mutationKey: [deleteUserMutationKey, token],
        mutationFn: async (id: string) => {
            const result = await getFactory(token).apiUserDeleteIdDelete({id});
            await queryClient.invalidateQueries({queryKey: [getUsersQueryKey], type: "all"});  // If the form submission succeeds then some other queries need to be refresh so invalidate them to do a refresh.
            await queryClient.invalidateQueries({queryKey: [getUserQueryKey], type: "all"});

            return result;
        }
    })
}