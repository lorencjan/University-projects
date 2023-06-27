import { ISportGroundItem } from '../../pages/sport-grounds';
import { Typography } from '@mui/material';


interface ISportGroundDetailProps {
    sportGround: ISportGroundItem
}

const SportGroundDetail: React.FC<ISportGroundDetailProps> = ({ sportGround }) => {
    return <>
        <Typography variant='h6'>{sportGround.nazev}</Typography>
        <Typography>{sportGround.adresa}</Typography>
        <Typography>{sportGround.typ_sportoviste_nazev}</Typography>
        <Typography>{sportGround.url}</Typography>
    </>
}

export default SportGroundDetail;