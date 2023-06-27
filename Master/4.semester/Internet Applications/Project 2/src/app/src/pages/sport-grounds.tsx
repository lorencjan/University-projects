import * as React from 'react';
import { useState, useEffect } from 'react';
import { GridColDef } from '@mui/x-data-grid';
import { Box, Autocomplete, TextField } from '@mui/material';
import SearchField from '../controls/search-field';
import Table from '../controls/table';
import download_dataset from '../data/downloader';
import { IInstitutionItem, InstitutionKind } from '../controls/maps/institution-marker';
import Accidents from '../controls/accidents';
import '../styles/institutions.css';

export interface ISportGroundItem extends IInstitutionItem {
    kind: InstitutionKind.SPORTGROUND;
    nazev: string;
    typ_sportoviste_nazev: string;
    adresa: string;
    url: string;
    X: number;
    Y: number;
}

const columns: GridColDef[] = [
    {
        field: 'nazev',
        headerName: 'Name',
        flex: 0.25
    },
    {
        field: 'typ_sportoviste_nazev',
        headerName: 'Type',
        flex: 0.25
    },
    {
        field: 'adresa',
        headerName: 'Address',
        flex: 0.25
    },
    {
        field: 'url',
        headerName: 'Web',
        flex: 0.25
    }
];

const SportGrounds: React.FC = () => {
    const [allSportGrounds, setAllSportGrounds] = useState<ISportGroundItem[]>([]);
    const [filteredSportGrounds, setFilteredSportGrounds] = useState<ISportGroundItem[]>([]);
    const [selectedSportGround, setSelectedSportGround] = useState<ISportGroundItem | null>(null);
    const [searchFilter, setSearchFilter] = useState<string | null>(null);
    const [typeFilter, setTypeFilter] = useState<(string | null)[]>([]);
    const typeFilterOptions = allSportGrounds
        .map(x => x.typ_sportoviste_nazev)              // select type
        .filter((x, i, arr) => arr.indexOf(x) === i)    // distinct operation
    typeFilterOptions.sort();

    useEffect(() => {
        (async function download() {
            let data = await download_dataset<ISportGroundItem>('http://localhost:5118/sport-grounds');
            data = data.map((item, idx) => ({ ...item, id: idx, kind: InstitutionKind.SPORTGROUND }))
            setAllSportGrounds(data);
            setFilteredSportGrounds(data);
        })();
    }, []);

    const filter = (searchText: string | null, newTypeFilter: (string | null)[] | undefined = undefined) => {
        newTypeFilter ??= typeFilter;
        const searchCmp = (x: string) => !searchText || (x && x.toLowerCase().includes(searchText.toLowerCase()));
        const filtered = allSportGrounds
            .filter(x => (searchCmp(x.nazev) || searchCmp(x.typ_sportoviste_nazev) || searchCmp(x.adresa) || searchCmp(x.url)) &&  // full-text search
                         (!newTypeFilter?.length || newTypeFilter.some(f => f === x.typ_sportoviste_nazev)));                      // type
        
        setSearchFilter(searchText);
        setTypeFilter(newTypeFilter);
        setFilteredSportGrounds(filtered);
    };

    return <>
        <Box className='institution-filter'>
            <Autocomplete
                multiple
                options={typeFilterOptions}
                onChange={(_, newValue) => filter(searchFilter, newValue)}
                size='small'
                renderInput={(params) => <TextField {...params} label='Type' />}
            />
            <SearchField onChange={filter} />
        </Box>
        <Table columns={columns} rows={filteredSportGrounds} onRowClick={row => setSelectedSportGround(row as ISportGroundItem)} />
        <Accidents institutions={filteredSportGrounds} selected={selectedSportGround} />
    </>;
}

export default SportGrounds;