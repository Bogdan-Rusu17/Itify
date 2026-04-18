import {Paper, Table, TableBody, TableCell, TableContainer, TableHead, TableRow} from "@mui/material";
import {DataTableProps} from "./DataTable.types";

export function DataTable<T>(props: DataTableProps<T>) {
    const {header, extraHeader, data} = props;

    return <TableContainer component={Paper}>
        <Table>
            <TableHead>
                <TableRow>
                    {
                        header.map(e => {
                            return {
                                order: e.order,
                                key: String(e.key),
                                name: e.name
                            }
                        }).concat((extraHeader ?? []).map(e => {
                            return {
                                order: e.order,
                                key: e.key,
                                name: e.name
                            }
                        }))
                            .sort((a, b) => a.order - b.order)
                            .map(e => <TableCell key={`header_${String(e.key)}`}>{e.name}</TableCell>)
                    }
                </TableRow>
            </TableHead>
            <TableBody>
                {
                    data.map((entry, rowIndex) => <TableRow key={`row_${rowIndex + 1}`}>
                        {header.map(e => {
                            return {
                                order: e.order,
                                key: String(e.key),
                                element: e.render ? e.render(entry[e.key], entry) : entry[e.key]?.toString()
                            }
                        }).concat((extraHeader ?? []).map(e => {
                            return {
                                order: e.order,
                                key: e.key,
                                element: e.render(entry)
                            }
                        }))
                            .sort((a, b) => a.order - b.order)
                            .map((keyValue) => <TableCell key={`cell_${rowIndex + 1}_${String(keyValue.key)}`}>{
                                keyValue.element
                            }</TableCell>)}
                    </TableRow>)
                }
            </TableBody>
        </Table>
    </TableContainer>
}