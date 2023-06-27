import { IAccidentItem } from '../accidents';
import { Typography } from '@mui/material';

interface AccidentDetailProps {
    accident: IAccidentItem
}

const AccidentDetail: React.FC<AccidentDetailProps> = ({ accident }) => {
    return <>
        <Typography variant='h6'>{accident.srazka}</Typography>
        <Typography>{accident.getDate().toLocaleString()}</Typography>
        <Typography>{accident.hlavni_pricina}</Typography>
        <Typography>{accident.pricina}</Typography>
        <Typography>{accident.situace_nehody}</Typography>
        <Typography>{[accident.chodec_pohlavi, accident.vek_skupina].filter(Boolean).join(', ')}</Typography>
        <Typography>{accident.nasledky_chodce}</Typography>
    </>;
}

export default AccidentDetail;