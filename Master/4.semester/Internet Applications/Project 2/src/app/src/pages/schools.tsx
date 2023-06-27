import * as React from 'react';
import { useState, useEffect } from 'react';
import { GridColDef } from '@mui/x-data-grid';
import { Box, Autocomplete, TextField } from '@mui/material';
import SearchField from '../controls/search-field';
import Table from '../controls/table';
import Accidents from '../controls/accidents';
import download_dataset from '../data/downloader';
import { IInstitutionItem, InstitutionKind } from '../controls/maps/institution-marker';
import '../styles/institutions.css';

export interface ISchoolItem extends IInstitutionItem {
    kind: InstitutionKind.SCHOOL;
    nazev_cely: string;
    zs: string;
    naz_cobce: string;
    X: number;
    Y: number;
}

const columns: GridColDef[] = [
    {
        field: 'nazev_cely',
        headerName: 'School name',
        flex: 0.5
    },
    {
        field: 'zs',
        headerName: 'Address',
        flex: 0.25
    },
    {
        field: 'naz_cobce',
        headerName: 'District',
        flex: 0.25
    }
];

const Schools: React.FC = () => {
    const [allSchools, setAllSchools] = useState<ISchoolItem[]>([]);
    const [filteredSchools, setFilteredSchools] = useState<ISchoolItem[]>([]);
    const [selectedSchool, setSelectedSchool] = useState<ISchoolItem | null>(null);
    const [searchFilter, setSearchFilter] = useState<string | null>(null);
    const [districtFilter, setDistrictFilter] = useState<(string | null)[]>([]);
    const districtFilterOptions = allSchools
        .map(x => x.naz_cobce)                          // select district
        .filter((x, i, arr) => arr.indexOf(x) === i);   // distinct operation
    districtFilterOptions.sort();

    useEffect(() => {
        (async function download() {
            let data = await download_dataset<ISchoolItem>('http://localhost:5118/schools');
            data = data.map((item, idx) => ({ ...item, id: idx, kind: InstitutionKind.SCHOOL }))
            setAllSchools(data);
            setFilteredSchools(data);
        })();
    }, []);

    const filter = (searchText: string | null, newDistrictFilter: (string | null)[] | undefined = undefined) => {
        newDistrictFilter ??= districtFilter;
        const searchCmp = (x: string) => !searchText || (x && x.toLowerCase().includes(searchText.toLowerCase()));
        const filtered = allSchools
            .filter(x => (searchCmp(x.nazev_cely) || searchCmp(x.zs) || searchCmp(x.naz_cobce)) &&         // full-text search
                         (!newDistrictFilter?.length || newDistrictFilter.some(f => f === x.naz_cobce)));  // district
        
        setSearchFilter(searchText);
        setDistrictFilter(newDistrictFilter);
        setFilteredSchools(filtered);
    };

    return <>
        <Box className='institution-filter'>
            <Autocomplete
                multiple
                options={districtFilterOptions}
                onChange={(_, newValue) => filter(searchFilter, newValue)}
                size='small'
                renderInput={(params) => <TextField {...params} label='District' />}
            />
            <SearchField onChange={filter} />
        </Box>
        <Table columns={columns} rows={filteredSchools} onRowClick={row => setSelectedSchool(row as ISchoolItem | null)} />
        <Accidents institutions={filteredSchools} selected={selectedSchool} />
    </>;
}

export default Schools;