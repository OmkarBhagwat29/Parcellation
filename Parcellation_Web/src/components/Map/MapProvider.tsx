import { Map } from "leaflet";
import React, { createContext, useState, useContext, ReactNode } from "react";
import { MapGeometry } from "./mapGeometry";

interface Coordinates {
  lat: number;
  lng: number;
}

interface MapContextType {
  coordinates: Coordinates;
  setCoordinates: (coordinates: Coordinates) => void;
  map: Map | null;
  setMap: (map: Map | null) => void;
  geometry: MapGeometry;
  setGeometry: (geometry: MapGeometry) => void;
  isExtracting: boolean;
  setIsExtracting: (isLoading: boolean) => void;
}

const MapContext = createContext<MapContextType | undefined>(undefined);

export const MapProvider = ({ children }: { children: ReactNode }) => {
  const [coordinates, setCoordinates] = useState<Coordinates>({
    lat: 51.44872354272485, // default lat
    lng: 7.026855865003494, // default lng

    //51.44872354272485, 7.026855865003494
  });

  const [map, setMap] = useState<Map | null>(null);

  const [isExtracting, setIsExtracting] = useState<boolean>(false);

  const [geometry, setGeometry] = useState<MapGeometry | null>(null);

  return (
    <MapContext.Provider
      value={{
        coordinates,
        setCoordinates,
        map,
        setMap,
        geometry,
        setGeometry,
        isExtracting,
        setIsExtracting,
      }}
    >
      {children}
    </MapContext.Provider>
  );
};

export const useMap = (): MapContextType => {
  const context = useContext(MapContext);
  if (!context) {
    throw new Error("useMap must be used within a MapProvider");
  }
  return context;
};
