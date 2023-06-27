import * as React from 'react';
import { Box, CircularProgress } from '@mui/material';


const Launch: React.FC = () => 
    <Box sx={{ height: '100vh', display: 'flex', justifyContent: 'center', alignItems: 'center' }}>
        <CircularProgress />
    </Box>


export default Launch;