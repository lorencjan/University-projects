import { useMemo, useState } from 'react';
import { MarkerF, InfoWindowF, CircleF } from '@react-google-maps/api';
import { ISchoolItem } from '../../pages/schools';
import { ISportGroundItem } from '../../pages/sport-grounds';
import { PointIdentifier, IIndentifiablePoint, Meters } from './google-maps';
import SchoolDetail from './school-detail';
import SportGroundDetail from './sportground-detail';

const schoolMarkerIcon: string = require('./../../assets/school-marker.svg').default;
const sportMarkerIcon: string = require('./../../assets/sport-marker.svg').default;

export enum InstitutionKind { SCHOOL, SPORTGROUND }
export interface IInstitutionItem extends IIndentifiablePoint {
    kind: InstitutionKind;
    X: number;
    Y: number;
}

interface IInstitutionMarkerProps {
    id: PointIdentifier;
    institution: IInstitutionItem;
    radius: Meters;
    clusterer?: any;
    showInstitutionDetail: PointIdentifier | null;
    onClick: (id: PointIdentifier) => void ;
}

export const isHiddenTitle = 'hidden';

const InstitutionMarker: React.FC<IInstitutionMarkerProps> = ({ id, institution, radius, clusterer, showInstitutionDetail, onClick }) => {
    const center = useMemo(() => new google.maps.LatLng(institution.Y, institution.X), [institution]);
    const [marker, setMarker] = useState<google.maps.Marker | undefined>();
    const [isVisible, setIsVisible] = useState<boolean>(true);

    const markerTitleChangedHandler = () => {
        if (marker) {
            const title = marker.getTitle();
            setIsVisible(title !== isHiddenTitle);
        }
    }

    const institutionDetail = (institution: IInstitutionItem) => {
        switch (institution.kind) {
            case InstitutionKind.SCHOOL:
                return <SchoolDetail school={institution as ISchoolItem} />;
            case InstitutionKind.SPORTGROUND:
                return <SportGroundDetail sportGround={institution as ISportGroundItem} />;
        }
    }

    const institutionMarkerIcon = (institution: IInstitutionItem) => {
        switch (institution.kind) {
            case InstitutionKind.SCHOOL:
                return schoolMarkerIcon;
            case InstitutionKind.SPORTGROUND:
                return sportMarkerIcon;
        }
    }

    return <MarkerF
                position={center}
                icon={{
                    url: institutionMarkerIcon(institution),
                }}
                clusterer={clusterer}
                onClick={() => onClick(id)}
                onLoad={(marker) => setMarker(marker)}
                onTitleChanged={markerTitleChangedHandler}
            >
                {showInstitutionDetail === id && <InfoWindowF position={center} zIndex={2}>
                    {institutionDetail(institution)}
                </InfoWindowF>}
                {isVisible && <CircleF center={center} radius={radius} options={{ clickable: true, strokeOpacity: 0 }} />}
            </MarkerF>;
}

export default InstitutionMarker;