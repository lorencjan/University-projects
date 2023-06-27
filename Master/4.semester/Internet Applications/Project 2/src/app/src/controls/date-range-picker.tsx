import { useEffect, useMemo, useState } from 'react';
import { Box } from '@mui/material';
import { DatePicker, LocalizationProvider } from '@mui/x-date-pickers';
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';
import dayjs from 'dayjs';
import '../styles/date-range-picker.css';

export interface DateRange {
    startDate: Date | null;
    endDate: Date | null;
}

interface DateRangePickerProps {
    defaultValue?: DateRange
    onChange?: (startDate: Date | null, endDate: Date | null) => void
}

function fmap<T extends {}, R>(value: T | null | undefined, transformer: (value: T) => R): R | null {
    return value ? transformer(value) : null;
}

const DateRangePicker: React.FC<DateRangePickerProps> = ({ defaultValue, onChange }) => {
    const minDate = useMemo(() => dayjs('2010-01-01'), []);
    const today = dayjs(new Date());
    const defaultStartValue = useMemo(() => fmap(defaultValue?.startDate, dayjs), [defaultValue]);
    const defaultEndValue = useMemo(() => fmap(defaultValue?.endDate, dayjs), [defaultValue]);
    const [startDate, setStartDate] = useState<dayjs.Dayjs | null>(defaultStartValue);
    const [endDate, setEndDate] = useState<dayjs.Dayjs | null>(defaultEndValue);

    useEffect(() => onChange?.(startDate?.toDate() ?? null, endDate?.toDate() ?? null), [startDate, endDate]);

    return <Box className='date-range-picker'>
            <LocalizationProvider dateAdapter={AdapterDayjs}>
                <DatePicker
                    label='From'
                    minDate={minDate}
                    maxDate={endDate ? endDate : today}
                    defaultValue={defaultStartValue}
                    onAccept={(date) => setStartDate(date)}
                    onChange={(date) => fmap(date, (value) => setStartDate(value))}
                    slotProps={{
                        textField: { variant: 'outlined', size: 'small' }
                    }}
                />
                <Box className='date-range-picker-separator'>
                    <Box />
                </Box>
                <DatePicker
                    label='To'
                    minDate={startDate ? startDate : minDate}
                    maxDate={today}
                    defaultValue={defaultEndValue}
                    onAccept={(date) => setEndDate(date)}
                    onChange={(date) => fmap(date, (value) => setEndDate(value))}
                    slotProps={{
                        textField: { variant: 'outlined', size: 'small' }
                    }}
                />
            </LocalizationProvider>
        </Box>;
}

export default DateRangePicker;