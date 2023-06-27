import * as React from 'react';
import { useState } from 'react';
import { LoadScriptNext } from '@react-google-maps/api';
import Launch from './launch';
import { styled, useTheme } from '@mui/material/styles';
import {
    Typography, Box, Drawer, List, Divider, IconButton, ListItem,
    ListItemButton, ListItemIcon, ListItemText, Tooltip
} from '@mui/material';
import {
    Home as HomeIcon, SportsBasketball as SportsBasketballIcon, School as SchoolIcon,
    ChevronLeft as ChevronLeftIcon, ChevronRight as ChevronRightIcon
} from '@mui/icons-material';

interface IRootProps {
    content: JSX.Element;
}

const DrawerHeader = styled('div')(() => ({
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'flex-end',
}));

const googleMapsApiKey = 'AIzaSyD7VyFp2sgmdSExS9C7wR6wOLVkpbLydpM'
const googleMapsLibraries: any[] = ['visualization', 'geometry']

const Root: React.FC<IRootProps> = ({ content }) => {
    const theme = useTheme().palette;
    const [isNavOpened, setNavOpened] = useState(window.localStorage.getItem('isNavOpened') === 'true');
    const pagePadding = 32;
    const navWidth = isNavOpened ? 230 : 58;

    const toggleDrawer = () => {
        window.localStorage.setItem('isNavOpened', `${!isNavOpened}`);
        setNavOpened(!isNavOpened)
    }

    const drawerItems = [
        { name: 'Home', icon: <HomeIcon />, href: '/' },
        { name: 'Schools', icon: <SchoolIcon />, href: '/schools' },
        { name: 'Sport Grounds', icon: <SportsBasketballIcon />, href: '/sport-grounds' }
    ];

    return <>
        <LoadScriptNext
            googleMapsApiKey={googleMapsApiKey}
            libraries={googleMapsLibraries}
            loadingElement={<Launch />}
        ><>
            <Drawer variant='permanent' open={isNavOpened} sx={{ '& > .MuiPaper-root': { background: theme.secondary.main } }}>
                <DrawerHeader>
                    <IconButton onClick={toggleDrawer}>
                        {isNavOpened ? <ChevronLeftIcon /> : <ChevronRightIcon />}
                    </IconButton>
                </DrawerHeader>
                <Divider />
                <List>
                    {drawerItems.map((item) => (
                        <ListItem key={item['name']} disablePadding sx={{ display: 'block', width: navWidth, overflow: 'hidden' }}>
                            <ListItemButton onClick={_ => window.location.href = item['href']} selected={item['href'] === window.location.pathname}>
                                <ListItemIcon sx={{ color: theme.primary.main }}>
                                    {isNavOpened ? item['icon'] : <Tooltip placement='right' title={item['name']}>{item['icon']}</Tooltip>}
                                </ListItemIcon>
                                <ListItemText primary={item['name']} sx={{ opacity: isNavOpened ? 1 : 0, whiteSpace: 'nowrap' }} />
                            </ListItemButton>
                        </ListItem>
                    ))}
                </List>
            </Drawer>
            <Box sx={{ marginLeft: `${navWidth}px`, width: `calc(100% - ${navWidth})` }}>
                <Typography variant='h6' sx={{ marginTop: '6px', paddingLeft: `${pagePadding}px`, paddingBottom: '2px', color: theme.primary.main }}>
                    {drawerItems.find(x => x['href'] === window.location.pathname)?.name ?? 'Error 404'}
                </Typography>
                <Divider></Divider>
                <Box sx={{ padding: `${pagePadding / 2}px ${pagePadding}px` }}>
                    {content}
                </Box>
            </Box>
        </></LoadScriptNext>
    </>;
}

export default Root;
