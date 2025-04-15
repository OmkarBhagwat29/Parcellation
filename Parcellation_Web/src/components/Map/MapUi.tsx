import React, { useEffect } from "react";

import { MapProvider } from "./MapProvider";
import MapTools from "./MapTools";
import MapViewerWrapper from "./MapViewerWrapper";
import MapVisibilityParams from "./MapVisibilityParams";
import MapExtraction from "./MapExtraction";
import MapLegends from "./MapLegends";

const MapUi = () => {
  //disable context menu
  useEffect(() => {
    const handleContextMenu = (event) => {
      event.preventDefault(); // Prevent the context menu
    };

    window.addEventListener("contextmenu", handleContextMenu);

    return () => {
      window.removeEventListener("contextmenu", handleContextMenu);
    };
  }, []);

  return (
    <MapProvider>
      <div id="main" className="absolute p-2">
        <div className="w-96 gap-2 flex flex-col">
          <MapViewerWrapper />
          <MapTools />
        </div>

        <div>
          <MapVisibilityParams />
        </div>

        <MapExtraction />

        <MapLegends />
      </div>
    </MapProvider>
  );
};

export default MapUi;
