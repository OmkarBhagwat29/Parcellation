import React, { useEffect } from "react";
import { useMap } from "../MapProvider";
import MapLegends from "./MapLegends";
import MapPropertiesTab from "./MapPropertiesTab";
import { sendToWebView } from "@/webview/webview";

import { useTheme } from "@/context/ThemeContext";
import { themes } from "@/theme";
import { AiOutlineClose } from "react-icons/ai";

const MapShowTab = () => {
  const { theme } = useTheme();
  const currentTheme = themes[theme];

  const { selectedTab, setSelectedTab, appDimensions } = useMap();

  useEffect(() => {
    console.log("resising");
    sendToWebView({ id: "map", command: "RESIZE" });
  }, [selectedTab]);

  return (
    <>
      {selectedTab !== "" && (
        <div
          className={`relative ${currentTheme.background} `}
          style={{
            width: `${250}px`,
          }}
        >
          <div className="absolute flex flex-col rounded-lg p-2 h-full w-full pb-6">
            {selectedTab === "Legends" && <MapLegends />}
            {selectedTab === "Properties" && <MapPropertiesTab />}
          </div>

          {/* Hide button (close icon) */}
          <button
            onClick={() => {
              setSelectedTab("");
              sendToWebView({ id: "map", command: "RESIZE" });
            }}
            className={`absolute bottom-2 right-2 p-1 rounded-full ${currentTheme.button} cursor-pointer`}
            title="Hide"
          >
            <AiOutlineClose size={20} />
          </button>
        </div>
      )}
    </>
  );
};

export default MapShowTab;
