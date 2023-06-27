import { ISchoolItem } from '../../pages/schools';
import { Typography } from '@mui/material';

interface ISchoolDetailProps {
    school: ISchoolItem
}

const SchoolDetail: React.FC<ISchoolDetailProps> = ({ school }) => {
    return <>
        <Typography variant='h6'>{school.nazev_cely}</Typography>
        <Typography>{[school.zs, school.naz_cobce].filter(Boolean).join(', ')}</Typography>
    </>;
}

export default SchoolDetail;