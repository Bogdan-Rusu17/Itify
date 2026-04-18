import {useIntl} from "react-intl";
import {isUndefined} from "lodash";
import {IconButton, TablePagination} from "@mui/material";
import {DataLoadingContainer} from "../../LoadingDisplay";
import {useUserTableController} from "./UserTable.controller";
import {UserRecord} from "@infrastructure/apis/client";
import DeleteIcon from '@mui/icons-material/Delete';
import {UserAddDialog} from "../../Dialogs/UserAddDialog";
import {useAppSelector} from "@application/store";
import {DataTable} from "@presentation/components/ui/Tables/DataTable";

/**
 * This hook returns a header for the table with translated columns.
 */
const useHeader = (): { key: keyof UserRecord, name: string, order: number }[] => {
    const {formatMessage} = useIntl();

    return [
        {key: "name", name: formatMessage({id: "globals.name"}), order: 1},
        {key: "email", name: formatMessage({id: "globals.email"}), order: 2},
        {key: "role", name: formatMessage({id: "globals.role"}), order: 3}
    ]
};

/**
 * The values in the table are organized as rows so this function takes the entries and creates the row values ordering them according to the order map.
 */
const getRowValues = (entries: UserRecord[] | null | undefined, orderMap: { [key: string]: number }) =>
    entries?.map(
        entry => {
            return {
                entry: entry,
                data: Object.entries(entry).filter(([e]) => !isUndefined(orderMap[e])).sort(([a], [b]) => orderMap[a] - orderMap[b]).map(([key, value]) => { return {key, value} })
            }
        });

/**
 * Creates the user table.
 */
export const UserTable = () => {
    const {userId: ownUserId} = useAppSelector(x => x.profileReducer);
    const {formatMessage} = useIntl();
    const header = useHeader();
    const {handleChangePage, handleChangePageSize, pagedData, isError, isLoading, tryReload, labelDisplay, remove} = useUserTableController(); // Use the controller hook.

    return <DataLoadingContainer isError={isError} isLoading={isLoading} tryReload={tryReload}> {/* Wrap the table into the loading container because data will be fetched from the backend and is not immediately available.*/}
        <UserAddDialog/> {/* Add the button to open the user add modal. */}
        {!isUndefined(pagedData) && !isUndefined(pagedData?.totalCount) && !isUndefined(pagedData?.page) && !isUndefined(pagedData?.pageSize) &&
            <TablePagination // Use the table pagination to add the navigation between the table pages.
                component="div"
                count={pagedData.totalCount} // Set the entry count returned from the backend.
                page={pagedData.totalCount !== 0 ? pagedData.page - 1 : 0} // Set the current page you are on.
                onPageChange={handleChangePage} // Set the callback to change the current page.
                rowsPerPage={pagedData.pageSize} // Set the current page size.
                onRowsPerPageChange={handleChangePageSize} // Set the callback to change the current page size. 
                labelRowsPerPage={formatMessage({id: "labels.itemsPerPage"})}
                labelDisplayedRows={labelDisplay}
                showFirstButton
                showLastButton
            />}

        <DataTable data={pagedData?.data ?? []}
                   header={header}
                   extraHeader={[{
                       key: "actions",
                       name: formatMessage({id: "labels.actions"}),
                       render: entry => <>
                           {entry.id !== ownUserId && <IconButton color="error" onClick={() => remove(entry.id || '')}>
                               <DeleteIcon color="error" fontSize="small"/>
                           </IconButton>
                           }</>,
                       order: 4
                   }]}
        />
    </DataLoadingContainer>
}