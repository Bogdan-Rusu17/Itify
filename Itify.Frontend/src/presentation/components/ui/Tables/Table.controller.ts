import { LabelDisplayedRowsArgs } from "@mui/material";
import { ChangeEvent, MouseEvent, useCallback } from "react";
import { useIntl } from "react-intl";

/**
 * This is the pagination controller hook that can be used to manage the state of a table.
 */
export const useTableController = (onPaginationChange: (page: number, pageSize: number) => void, defaultPageSize?: number) => {
    const { formatMessage } = useIntl();
    const handleChangePage = useCallback(( // Create a callback to listen on changes of the table page.
        _event: MouseEvent<HTMLButtonElement> | null,
        newPage: number,
    ) => {
        onPaginationChange(newPage + 1, defaultPageSize ?? 10);
    }, [onPaginationChange, defaultPageSize]);

    const handleChangePageSize = useCallback(( // Create a callback to listen on changes of the table page size.
        event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>,
    ) => {
        onPaginationChange(1, parseInt(event.target.value, 10)); // Reset the current page to 1 on page size change to avoid overflow.
    }, [onPaginationChange]);

    const labelDisplay = useCallback(({ to, from, count }: LabelDisplayedRowsArgs) => { // Create a callback to display the paging labels with translations.
        return count !== -1 ?
            formatMessage({ id: "labels.paginationLabelNormal" }, { to, from, count }) :
            formatMessage({ id: "labels.paginationLabelOverflow" }, { to, from });
    }, [formatMessage]);

    return {
        labelDisplay,
        handleChangePage,
        handleChangePageSize
    };
}