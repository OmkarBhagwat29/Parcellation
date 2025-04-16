import React, { useEffect } from "react";

import { MapProvider, useMap } from "./MapProvider";
import MapTools from "./MapTools";
import MapViewerWrapper from "./MapViewerWrapper";
import MapVisibilityParams from "./MapVisibilityParams";
import useAppSize from "./hooks/useAppSize";
import MapTabs from "./tabs/MapTabs";
import { useTheme } from "@/context/ThemeContext";
import { themes } from "@/theme";
import MapLegends from "./tabs/MapLegends";
import MapShowTab from "./tabs/MapShowTab";

const UiWrapper = () => {
  const { appDimensions, geometry, selectedTab } = useMap();
  useAppSize();

  const { theme } = useTheme();
  const currentTheme = themes[theme];

  return (
    <>
      <div
        id="main"
        className={`absolute pt-2 pl-2 pr-2 overflow-auto ${currentTheme.background}`}
      >
        <div
          className="flex flex-row gap-2 rounded-lg"
          style={{
            maxHeight: `${1000}px`,
          }}
        >
          <div
            className="flex flex-col w-full"
            style={{ width: `${appDimensions.width}px` }}
          >
            <div
              className="flex flex-col"
              style={{
                height: `${appDimensions.height}px`,

                width: `${appDimensions.width}px`,
              }}
            >
              <div className="flex-grow overflow-hidden shadow-xl">
                <MapViewerWrapper />
              </div>

              <div className="p-4 px-6">
                <MapTools />
              </div>
            </div>

            {geometry && (
              <div className=" px-6 pb-4">
                <MapVisibilityParams />
              </div>
            )}

            <div className="mt-auto">
              <MapTabs />
            </div>
          </div>

          <MapShowTab />
        </div>
      </div>
    </>
  );
};

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
      <UiWrapper />
    </MapProvider>
  );
};

export default MapUi;
