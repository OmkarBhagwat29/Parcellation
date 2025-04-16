import React from "react";

import MapSearch from "./MapSearch";
import dynamic from "next/dynamic";
import { useMap } from "./MapProvider";
import { useTheme } from "@/context/ThemeContext";
import { themes } from "@/theme";

const MapViewer = dynamic(() => import("./MapViewer"), {
  ssr: false,
});

const MapViewerWrapper = () => {
  const { isExtracting } = useMap();

  const { theme } = useTheme();
  const currentTheme = themes[theme];

  return (
    <div className="relative w-full h-full rounded-xl overflow-hidden shadow-lg">
      {/* Map background */}
      <div className={`absolute inset-0 z-0 ${isExtracting ? "blur-sm" : ""}`}>
        <MapViewer />
      </div>

      {/* Overlay: heading + search */}
      <div className="absolute top-0 left-0 z-10 w-full p-2 h-10">
        <MapSearch />
      </div>

      {/* Extracting overlay */}
      {isExtracting && (
        <div
          className={`absolute inset-0 z-20 flex items-center justify-center  opacity-80 ${currentTheme}`}
        >
          <div className={`${currentTheme.text} text-xl font-semibold`}>
            Processing...
          </div>
        </div>
      )}
    </div>
  );
};

export default MapViewerWrapper;
