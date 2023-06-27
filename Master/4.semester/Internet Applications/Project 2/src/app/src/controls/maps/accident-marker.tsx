import { useMemo } from 'react';
import { MarkerF, InfoWindowF } from '@react-google-maps/api';
import { PointIdentifier } from './google-maps';
import { IAccidentItem } from '../accidents';
import AccidentDetail from './accident-detail';

interface IAccidentMarkerProps {
    id: PointIdentifier;
    accident: IAccidentItem;
    clusterer?: any;
    showAccidentDetail: PointIdentifier | null;
    onClick: (id: PointIdentifier) => void ;
}

const AccidentMarker: React.FC<IAccidentMarkerProps> = ({ id, accident, clusterer, showAccidentDetail, onClick }) => {
    const center = useMemo(() => new google.maps.LatLng(accident.Y, accident.X), [accident]);

    return <MarkerF
        position={center}
        icon={{
            path: google.maps.SymbolPath.CIRCLE,
            scale: 3,
            fillColor: 'red',
            fillOpacity: 1,
            strokeColor: 'red'
        }}
        clusterer={clusterer}
        onClick={() => onClick(id)}
    >
        {showAccidentDetail === id &&
        <InfoWindowF position={center} zIndex={3}>
            <AccidentDetail accident={accident} />
        </InfoWindowF>}
    </MarkerF>;
}

export default AccidentMarker;