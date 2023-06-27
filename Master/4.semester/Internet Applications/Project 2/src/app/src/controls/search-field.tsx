import * as React from 'react';
import { useState } from 'react';
import { TextField, InputAdornment } from '@mui/material';
import { Search as SearchIcon, Clear as ClearIcon } from '@mui/icons-material';

interface ISearchFieldProps {
    onChange: (text: string) => void;
}

const SearchField: React.FC<ISearchFieldProps> = ({ onChange }) => {
    const [value, setValue] = useState('');
    const [showClearIcon, setShowClearIcon] = useState('none');

    const changeHandler = (event: React.ChangeEvent<HTMLInputElement>): void => {
        const text = event.target.value;
        setShowClearIcon(text ? 'flex' : 'none');
        setValue(text);
        onChange(text);
    };

    const clearHandler = (): void => {
        setShowClearIcon('none');
        setValue('');
        onChange('');
    };

    return <TextField
        value={value}
        size='small'
        variant='outlined'
        onChange={changeHandler}
        sx={{width: '250px'}}
        InputProps={{
            startAdornment: (
                <InputAdornment position='start'>
                    <SearchIcon />
                </InputAdornment>
            ),
            endAdornment: (
                <InputAdornment position='end' sx={{ display: showClearIcon, '&:hover': {cursor: 'pointer'} }} onClick={clearHandler}>
                    <ClearIcon />
                </InputAdornment>
            )
        }}
    />;
}

export default SearchField;