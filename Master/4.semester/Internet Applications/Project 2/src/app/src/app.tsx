import * as React from 'react';
import { createTheme, ThemeProvider } from '@mui/material/styles';
import themeOptions from './styles/theme';
import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import Root from './pages/root';
import Page404 from './pages/page-404';
import Home from './pages/home';
import Schools from './pages/schools';
import SportGrounds from './pages/sport-grounds';

const router = createBrowserRouter([
    {
        path: '/',
        element: <Root content={<Home />} />,
        errorElement: <Root content={<Page404 />} />
    },
    {
        path: '/schools',
        element: <Root content={<Schools />} />,
    },
    {
        path: '/sport-grounds',
        element: <Root content={<SportGrounds />} />,
    }
]);

const App: React.FC = () =>
    <ThemeProvider theme={createTheme(themeOptions)}>
        <RouterProvider router={router} />
    </ThemeProvider>

export default App;
