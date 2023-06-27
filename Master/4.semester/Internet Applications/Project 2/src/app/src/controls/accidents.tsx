import { useState, useEffect, useMemo } from 'react';
import download_dataset from '../data/downloader';
import GoogleMaps, { IIndentifiablePoint, Meters } from './maps/google-maps';
import { IInstitutionItem } from './maps/institution-marker';
import { Box, InputLabel, MenuItem, Select, SelectChangeEvent, Checkbox, FormControlLabel } from '@mui/material';
import DateRangePicker, { DateRange } from './date-range-picker';
import Charts from './charts'
import '../styles/accidents.css';

export interface IAccidentItem extends IIndentifiablePoint {
    srazka: string;
    cas: number;
    den: number;
    mesic: number;
    rok: number;
    hlavni_pricina: string;
    pricina: string;
    situace_nehody: string;
    vek_skupina: string;
    chodec_pohlavi: string;
    nasledky_chodce: string;
    X: number;
    Y: number;

    getDate: () => Date;
}

export interface IFilteredData {
    institutions: IInstitutionItem[];
    accidents: IAccidentItem[];
    selectedInstitution: IInstitutionItem | null;
}

interface IAccidentsProps {
    institutions: IInstitutionItem[];
    selected?: IInstitutionItem | null;
}

const getAccidentDate = (accident: IAccidentItem) => {
    const hours = Math.floor((accident.cas/100));
    const minutes = accident.cas % 100;
    return new Date(accident.rok, accident.mesic, accident.den, hours, minutes);
}

const dateRangeFilter = (accident: IAccidentItem, dateRange: DateRange) => {
    const accidentTimestamp = accident.getDate().getTime();
    const startTimestamp = dateRange.startDate?.getTime();
    const endTimestamp = dateRange.endDate?.getTime();
    const isAfterStartDate = startTimestamp ? startTimestamp <= accidentTimestamp : true;
    const isBeforeEndDate = endTimestamp ? accidentTimestamp <= endTimestamp : true;
    return isAfterStartDate && isBeforeEndDate;
}

const Accidents: React.FC<IAccidentsProps> = ({ institutions, selected }) => {
    const defaultDateRange = useMemo<DateRange>(() => ({ startDate: new Date(2019, 1, 1), endDate: new Date() }), []);
    const [distance, setDistance] = useState<Meters>(200);
    const [allAccidents, setAllAccidents] = useState<IAccidentItem[]>([]);
    const [filteredData, setFilteredData] = useState<IFilteredData>({ institutions: [], accidents: [], selectedInstitution: null });
    const [dateRange, setDateRange] = useState<DateRange>(defaultDateRange);
    const [showAll, setShowAll] = useState<boolean>(false);

    const selectChangeHandler = (event: SelectChangeEvent<Meters>) => setDistance(event.target.value as Meters);

    const showAllChangeHandler = (_: React.ChangeEvent<HTMLInputElement>, checked: boolean) => setShowAll(checked);

    const distanceFilter = (institutionLatLng: google.maps.LatLng, accident: IAccidentItem) => {
        const accidentLatLng = new google.maps.LatLng(accident.Y, accident.X);
        return google.maps.geometry.spherical.computeDistanceBetween(institutionLatLng, accidentLatLng) <= distance;
    }

    useEffect(() => {
        (async function download() {
            let data = await download_dataset<IAccidentItem>('http://localhost:5118/accidents');
            data = data.map((item, idx) => ({ ...item, id: idx, getDate: () => getAccidentDate(item) }));
            setAllAccidents(data);
        })();
    }, []);

    useEffect(() => {
        const selectedInstitutions = selected ? [selected] : [];
        const shownInstitutions = showAll ? institutions :  selectedInstitutions;
        const filteredAccidents = shownInstitutions.flatMap((institution) => {
            const institutionLatLng = new google.maps.LatLng(institution.Y, institution.X);
            return allAccidents.filter((accident) => (
                distanceFilter(institutionLatLng, accident) && dateRangeFilter(accident, dateRange)
            ));
        });
        // GoogleMaps component needs data to be updated all at once
        setFilteredData({ 
            institutions: shownInstitutions,
            accidents: filteredAccidents,
            selectedInstitution: selected ?? null
        });
    }, [institutions, selected, allAccidents, distance, dateRange, showAll]);

    return <>
        <Box className = "google-maps-wrapper">
            <Box className = "google-map-filter">
                <Box>
                    <InputLabel id='distance-select-label'>Distance</InputLabel>
                    <Select
                        labelId='distance-select-label'
                        variant='outlined'
                        size='small'
                        value={distance}
                        label='Distance'
                        onChange={selectChangeHandler}
                    >
                        <MenuItem value={50}>50m</MenuItem>
                        <MenuItem value={100}>100m</MenuItem>
                        <MenuItem value={200}>200m</MenuItem>
                        <MenuItem value={500}>500m</MenuItem>
                        <MenuItem value={1000}>1km</MenuItem>
                    </Select>
                </Box>
                <DateRangePicker
                    defaultValue={defaultDateRange}
                    onChange={(startDate, endDate) => setDateRange({ startDate, endDate })}
                />
                <FormControlLabel
                    label='Show all'
                    control={<Checkbox checked={showAll} onChange={showAllChangeHandler}/>}
                />
            </Box>
            <Box className = "google-map">
                <GoogleMaps data={filteredData} radius={distance} />
            </Box>
        </Box>
        <Charts filteredAccidents={filteredData} allAccidents={allAccidents} />
    </>;
}

export default Accidents;