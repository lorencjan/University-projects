import { useEffect, useMemo, useState } from 'react';
import { GoogleMap, MarkerClustererF } from '@react-google-maps/api';
import { IFilteredData } from '../accidents';
import InstitutionMarker, { isHiddenTitle } from './institution-marker';
import AccidentMarker from './accident-marker';

const institutionClusterMarker: string = require('./../../assets/institution-cluster-marker.svg').default;
const accidentClusterMarker: string = require('./../../assets/accident-cluster-marker.svg').default;

export type Meters = number;
export type PointIdentifier = string | number;

export interface IIndentifiablePoint {
    id: PointIdentifier;
    Y: number;
    X: number;
}

interface IGoogleMapsProps {
    data: IFilteredData;
    radius: Meters;
}

const GoogleMaps: React.FC<IGoogleMapsProps> = ({ data, radius }) => {
    const institutionsMinimumClusterSize = 70;
    const institutionsClusterGridSize = 60;
    const accidentsMinimumClusterSize = 40;
    const accidentsClusterGridSize = 60;
    const brnoCenter = useMemo(() => ({ lat: 49.2002210, lng: 16.6078410 }), []);
    const zoomToFitBrno = useMemo(() => 11, []);
    const [showInstitutionDetail, setShowInstitutionDetail] = useState<PointIdentifier | null>(null);
    const [showAccidentDetail, setShowAccidentDetail] = useState<PointIdentifier | null>(null);
    const [map, setMap] = useState<google.maps.Map | undefined>();


    const mapLoadHandler = (loadedMap: google.maps.Map) => setMap(loadedMap);

    const mapClickHandler = () => {
        setShowInstitutionDetail(null);
        setShowAccidentDetail(null);
    };

    const institutionClickHandler = (id: PointIdentifier) => {
        setShowInstitutionDetail(showInstitutionDetail === id ? null : id);
        setShowAccidentDetail(null);
    };

    const accidentClickHandler = (id: PointIdentifier) => setShowAccidentDetail(showAccidentDetail === id ? null : id);

    const extendBoundsByPadding = (bounds: google.maps.LatLngBounds, padding: Meters) => {
        const directionNorthEast = 45;
        const directionSouthWest = 180 + 45;
        const paddingNorthEast = google.maps.geometry.spherical.computeOffset(bounds.getNorthEast(), padding, directionNorthEast);
        const paddingSouthWest = google.maps.geometry.spherical.computeOffset(bounds.getSouthWest(), padding, directionSouthWest);
        bounds.extend(paddingNorthEast);
        bounds.extend(paddingSouthWest);
    };

    const fitPointsOnMap = (map: google.maps.Map, points: IIndentifiablePoint[], padding: Meters) => {
        const bounds = new google.maps.LatLngBounds();
        for (const point of points) {
            bounds.extend(new google.maps.LatLng(point.Y, point.X));
        }
        extendBoundsByPadding(bounds, padding);
        map.fitBounds(bounds);
    };

    useEffect(() => {
        if (data.institutions.length > 0 && map !== undefined) {
            if (data.selectedInstitution !== null) {
                fitPointsOnMap(map, [data.selectedInstitution], radius);
            } else {
                fitPointsOnMap(map, data.institutions, radius);
            }
        }  
    }, [map, data]);

    return <>
        <GoogleMap
            zoom={zoomToFitBrno}
            center={brnoCenter}
            options={{
                controlSize: 30,
                streetViewControl: false,
                clickableIcons: false
            }}
            mapContainerClassName='map-container'
            onLoad={mapLoadHandler}
            onClick={mapClickHandler}
        >
            <MarkerClustererF
                minimumClusterSize={institutionsMinimumClusterSize}
                gridSize={institutionsClusterGridSize}
                styles={[
                    {
                        url: institutionClusterMarker,
                        height: 40,
                        width: 40
                    }        
                ]}
                onClusteringBegin={(clusterer) => {
                    // Handles hiding of the clustered marker circle children, marks everything as visible
                    clusterer.getMarkers().forEach((marker) => marker.setTitle(undefined))
                }}
                onClusteringEnd={(clusterer) => {
                    // Handles hiding of the clustered marker circle children, marks clustered markers as hidden
                    const clustered = clusterer.clusters.flatMap((cluster) => {
                        const markers = cluster.getMarkers()
                        const isClustered = (markers.length >= institutionsMinimumClusterSize) 
                        return isClustered ? markers : []
                    })
                    clustered.forEach((marker) => marker.setTitle(isHiddenTitle))
                }}
            >
                {(clusterer) => <>
                    {data.institutions.map((institution, index) => <InstitutionMarker 
                        key={index}
                        id={index}
                        institution={institution}
                        radius={radius}
                        clusterer={clusterer}
                        showInstitutionDetail={showInstitutionDetail}
                        onClick={institutionClickHandler}
                    />)}
                </>}
            </MarkerClustererF>
            <MarkerClustererF
                minimumClusterSize={accidentsMinimumClusterSize}
                gridSize={accidentsClusterGridSize}
                styles={[
                    {
                        url: accidentClusterMarker,
                        height: 40,
                        width: 40
                    }        
                ]}
            >
                {(clusterer) => <>
                    {data.accidents.map((accident, index) => <AccidentMarker
                        key={index}
                        id={index}
                        accident={accident}
                        clusterer={clusterer}
                        showAccidentDetail={showAccidentDetail}
                        onClick={accidentClickHandler}
                    />)}
                </>}
            </MarkerClustererF>
        </GoogleMap>
    </>;
}

export default GoogleMaps;