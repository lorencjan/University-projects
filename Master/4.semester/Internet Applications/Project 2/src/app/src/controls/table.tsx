import * as React from 'react';
import { DataGrid, GridColDef, GridToolbar } from '@mui/x-data-grid';
import { IInstitutionItem } from '../controls/maps/institution-marker';

interface ITableProps {
    columns: GridColDef[];
    rows: object[];
    onRowClick?: (institution: IInstitutionItem) => void;
}

const Table: React.FC<ITableProps> = ({ columns, rows, onRowClick }) => {
    const onRowClickHandler = (params: any) => {
        const node = document.querySelector<HTMLElement>(`.MuiDataGrid-row[data-id='${params.id}']`);
        setTimeout(() => {onRowClick?.(node?.classList.contains('Mui-selected') ? params.row : null)}, 5);
    }
    
    return <DataGrid sx={{ 
            '& .MuiDataGrid-columnHeaderTitle': { fontWeight: 'bold' },
            '& .MuiDataGrid-row:hover': { cursor: 'pointer' }
        }}
        columns={columns}
        rows={rows}
        onRowClick={onRowClickHandler}
        pageSize={10}
        disableColumnMenu
        disableColumnFilter
        autoHeight
        density='compact'
        components={{ Toolbar: GridToolbar }}
        componentsProps={{
            toolbar: {
                quickFilterProps: { debounceMs: 500 },
            },
        }}
    />;
}

export default Table;