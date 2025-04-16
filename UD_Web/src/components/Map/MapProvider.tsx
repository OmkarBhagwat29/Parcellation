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

  setAppDimensions: ({
    width,
    height,
  }: {
    width: number;
    height: number;
  }) => void;
  appDimensions: { width: number; height: number };
  selectedTab: string;
  setSelectedTab: (tabName: string) => void;
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

  const [appDimensions, setAppDimensions] = useState<{
    width: number;
    height: number;
  }>({
    width: 400,
    height: 400,
  });

  const [selectedTab, setSelectedTab] = useState("");

  return (
    <MapContext.Provider
      value={{
        selectedTab,
        setSelectedTab,
        coordinates,
        setCoordinates,
        map,
        setMap,
        geometry,
        setGeometry,
        isExtracting,
        setIsExtracting,
        appDimensions,
        setAppDimensions,
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
