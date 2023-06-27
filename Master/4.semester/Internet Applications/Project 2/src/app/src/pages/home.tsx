import * as React from 'react';
import { Box, Typography } from '@mui/material';
import '../styles/home.css';

const Home: React.FC = () =>
    <Box className="home">
        <Box>
            <Typography variant='h5'>Hello there!</Typography>
            <Typography variant='h6'>
                If you're interested in Brno's primary schools and sport grounds and how they're safe respective to the traffic, you've come to the right place.
            </Typography>
            <Box component='img' src='img/home.jpg' />
        </Box>
    </Box>

export default Home;
