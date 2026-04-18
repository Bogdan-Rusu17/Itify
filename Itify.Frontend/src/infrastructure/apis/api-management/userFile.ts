import { useAppSelector } from "@application/store";
import {ApiUserFileAddPostRequest, Configuration, UserFileApi} from "../client";
import {useMutation, useQuery, useQueryClient} from "@tanstack/react-query";

/**
 * Use constants to identify mutations and queries.
 */
const getUserFilesQueryKey = "getUserFilesQuery";
const downloadUserFileQueryKey = "downloadUserFileQuery";
const addUserFileMutationKey = "addUserFileMutation";

const getFactory = (token: string | null) => new UserFileApi(new Configuration({accessToken: token ?? ""}));

export const useGetUserFiles = (page: number, pageSize: number, search: string = "") => {
    const {token} = useAppSelector(x => x.profileReducer); // You can use the data form the Redux storage.

    return {
        ...useQuery({
            queryKey: [getUserFilesQueryKey, token, page, pageSize, search],
            queryFn: async () => await getFactory(token).apiUserFileGetPageGet({page, pageSize, search}),
            refetchInterval: Infinity, // User information may not be frequently updated so refetching the data periodically is not necessary.
            refetchOnWindowFocus: false // This disables fetching the user information from the backend when focusing on the current window.
        }),
        queryKey: getUserFilesQueryKey
    };
}

export const useAddUserFile = () => {
    const { token } = useAppSelector(x => x.profileReducer); // You can use the data form the Redux storage.
    const queryClient = useQueryClient();

    return useMutation({
        mutationKey: [addUserFileMutationKey, token],
        mutationFn: async (userFile: ApiUserFileAddPostRequest) => {
            const result = await getFactory(token).apiUserFileAddPost(userFile);
            await queryClient.invalidateQueries({queryKey: [getUserFilesQueryKey], type: "all"});  // If the form submission succeeds then some other queries need to be refresh so invalidate them to do a refresh.

            return result;
        }
    })
}

export const useDownloadUserFile = () => {
    const { token } = useAppSelector(x => x.profileReducer); // You can use the data form the Redux storage.

    return useMutation({
        mutationKey: [downloadUserFileQueryKey, token],
        mutationFn: async (id: string) => {
            return await getFactory(token).apiUserFileDownloadIdGet({ id });
        }
    })
}