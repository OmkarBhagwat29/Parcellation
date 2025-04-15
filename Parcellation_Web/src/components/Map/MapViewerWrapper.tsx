import React from "react";

import MapSearch from "./MapSearch";
import dynamic from "next/dynamic";

const MapViewer = dynamic(() => import("./MapViewer"), {
  ssr: false,
});

const MapViewerWrapper = () => {
  return (
    <div className={`h-96 gap-2`}>
      <div className="relative w-full h-full rounded-xl overflow-hidden shadow-lg">
        {/* Map background */}
        <div className="absolute inset-0 z-0">
          <MapViewer />
        </div>

        {/* Overlay: heading + search */}
        <div className="absolute top-0 left-0 z-10 w-full p-2 h-10">
          <MapSearch />
        </div>
      </div>
    </div>
  );
};

export default MapViewerWrapper;
