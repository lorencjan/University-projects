import { Box, Typography } from '@mui/material';
import { PieChart, BarChart, Pie, Bar, XAxis, YAxis, CartesianGrid, Legend, Tooltip } from 'recharts';
import { IAccidentItem, IFilteredData } from './accidents';
import '../styles/charts.css';

interface IPieChartProps {
    title: string;
    data: any[];
}

interface IChartsProps {
    filteredAccidents: IFilteredData;
    allAccidents: IAccidentItem[];
}

function getPieChartData(accidents: IAccidentItem[]): [number[],number[]] {
    let children = 0;
    let adults = 0;
    let dead = 0;
    let injuries = 0;
    let uninjured = 0;
    const childrenAges = ["0-6", "7-11", "12-15", "16-18"];
    
    for(let accident of accidents)
    {   
        // count children vs adults
        if (childrenAges.includes(accident.vek_skupina))
            children++;
        else
            adults ++;
        
        // count injury severity
        if(accident.nasledky_chodce === "usmrcení")
            dead++;
        else if(accident.nasledky_chodce === "bez zranění")
            uninjured++;
        else
            injuries++;
    }
    let ageCategories: number[] = [children, adults];
    let injuryCategories : number[] = [dead, injuries, uninjured];

    return [ageCategories, injuryCategories];
}

function getInjuriesOverTime(accidents: IAccidentItem[]): number[][] {
    let injuryStats : number[][] = [];
    for(let i = 0; i < 2021 - 2010 + 1; i++)
        injuryStats.push([0,0,0]);
        
    for(let accident of accidents) {
        let stats = injuryStats[accident.rok - 2010];
        if(accident.nasledky_chodce === "usmrcení")
            stats[0]++;
        else if(accident.nasledky_chodce === "bez zranění")
            stats[2]++;
        else
            stats[1]++;
    }

    return injuryStats;
}

const PieChartWrapper: React.FC<IPieChartProps> = ({title, data}) =>
    <Box>
        <Typography variant="h6" className='chart-title'>{title}</Typography>
        <PieChart width={300} height={300}>
            <Pie
                data={data}
                cx="50%"
                cy="50%"
                innerRadius={50}
                outerRadius={100}
                dataKey="value"
            />
            <Tooltip />
            <Legend />
        </PieChart>
    </Box>

const Charts: React.FC<IChartsProps> = ({ filteredAccidents, allAccidents }) => {
    const [ageCategories, injurySeverities] = getPieChartData(filteredAccidents.accidents);
    const childrenAdultPieChartData = ageCategories.map((value, index) => ({
        name: index === 0 ? "Children" : "Adults",
        value,
        fill: ['blue', 'brown'][index % 2]
    }));
    const injurySeverityPieChartData = injurySeverities.map((value, index) => ({
        name: index === 0 ? "Death" : index === 1 ? "Injury" : "Uninjured",
        value,
        fill: ['black', 'orange', 'green'][index % 3]
    }));

    const barchart = getInjuriesOverTime(allAccidents);
    const barChartData: any[] | undefined = [];
    for(let i = 0; i < barchart.length; i++)
        barChartData.push({ name: (2010 + i).toString(10), Death:barchart[i][0], Injury:barchart[i][1], Uninjured:barchart[i][2] });

    return <Box className='charts-wrapper'>
        <Box className='total-accidents-chart'>
            <Typography variant="h6" className='chart-title'>Total accidents per year</Typography>
            <BarChart width={730} height={300} data={barChartData}>
                    <CartesianGrid strokeDasharray="3 3" />
                    <XAxis dataKey="name"/>
                    <YAxis />
                    <Tooltip />
                    <Legend />
                    <Bar dataKey="Death" fill='black' />
                    <Bar dataKey="Injury" fill='orange' />
                    <Bar dataKey="Uninjured" fill='green' />
            </BarChart>
        </Box>
        {Boolean(filteredAccidents.accidents.length) && (
        <Box className='pie-charts'>
            <PieChartWrapper title="Children to adults ratio" data={childrenAdultPieChartData}/>
            <PieChartWrapper title="Accident severity" data={injurySeverityPieChartData}/>
        </Box>
        )}
    </Box>;
}

export default Charts;