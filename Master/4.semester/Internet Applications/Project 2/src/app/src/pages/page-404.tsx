import * as React from 'react';
import { Box, Typography } from '@mui/material';
import '../styles/page404.css';

const Page404: React.FC = () =>
    <Box className="page-404">
        <Typography variant='h3'>Ooops...</Typography>
        <Typography variant='h3'>Page not found</Typography>
        <Box component='img' src='img/404.png'/>
    </Box>

export default Page404;
