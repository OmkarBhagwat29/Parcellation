import {
  MapContainer,
  MapContainerProps,
  TileLayer,
  ZoomControl,
} from "react-leaflet";

import { useMap as useLeafletMap } from "react-leaflet";

import "leaflet/dist/leaflet.css";
import { useEffect } from "react";
import L from "leaflet";
import { useMap } from "./MapProvider";

const MapViewer = () => {
  const { coordinates } = useMap();

  useEffect(() => {
    // Fix Leaflet icon issue
    L.Icon.Default.mergeOptions({
      iconRetinaUrl:
        "https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon-2x.png",
      iconUrl: "https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon.png",
      shadowUrl:
        "https://unpkg.com/leaflet@1.9.4/dist/images/marker-shadow.png",
    });
  }, []);

  return (
    <MapContainer
      key={`${coordinates.lat}-${coordinates.lng}`} // Add key to force re-render on coordinates change
      center={[coordinates.lat, coordinates.lng]} // This will update when coordinates change
      zoom={50}
      scrollWheelZoom={true}
      className="w-full h-full z-0"
      zoomControl={false} // disable default control
      attributionControl={true} // this is true by default, but we override its content
    >
      <MapSetter /> {/* Set the map instance in context */}
      {/* Tile layer */}
      <TileLayer
        attribution='&copy; <a href="http://osm.org/copyright">ombhagwat29@gmail.com</a>'
        url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
      />
      {/* Add zoom control at bottom-left */}
      <ZoomControl position="bottomleft" />
    </MapContainer>
  );
};

const MapSetter = () => {
  const leafletMap = useLeafletMap();
  const { setMap } = useMap();

  useEffect(() => {
    setMap(leafletMap); // Now you have the real Leaflet Map instance

    // Remove the "Leaflet" branding text
    leafletMap.attributionControl.setPrefix("omkar.bhagwat");
  }, [leafletMap]);

  return null;
};

export default MapViewer;
