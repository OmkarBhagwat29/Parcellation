import { useTheme } from "@/context/ThemeContext";
import { themes } from "@/theme";
import React from "react";
import Button from "../Inputs/Button";
import { useMap } from "./MapProvider";
import { getmapGeometry } from "./mapGeometry";
import { isWebViewMode, sendToWebView } from "@/webview/webview";

const MapTools = () => {
  const { theme } = useTheme();
  const currentTheme = themes[theme];
  const { map, setGeometry, setIsExtracting } = useMap();

  const handleExtract = async () => {
    console.log("Extracting map geometry..."); // Debugging line

    setIsExtracting(true); // Set loading state to true

    if (isWebViewMode()) {
      sendToWebView({
        id: "mapGeometry",
        command: "RESIZE",
        payload: null,
      });
    }

    const data = await getmapGeometry(map);

    setGeometry(data); // Update the geometry in the context

    setIsExtracting(false);

    if (isWebViewMode()) {
      sendToWebView({
        id: "mapGeometry",
        command: "MAP_GEOMETRY",
        payload: { value: data },
      });
    }
  };

  const handleClear = () => {
    setGeometry(null); // Clear the geometry in the context

    if (isWebViewMode()) {
      sendToWebView({
        id: "mapGeometry",
        command: "MAP_CLEAR",
        payload: null,
      });
    }
  };

  const handleSave = () => {
    if (isWebViewMode()) {
      sendToWebView({
        id: "mapGeometry",
        command: "MAP_SAVE",
        payload: null,
      });
    }
  };

  return (
    <div className="relative w-full flex flex-col gap-2">
      <div className="flex flex-row gap-2 justify-between items-center w-full">
        <div className="w-18 h-12 py-1">
          <Button
            onClick={handleExtract}
            name="Extract"
            className={`${currentTheme.button} w-full h-full`}
          />
        </div>
        <div className="w-18 h-12 py-1">
          <Button
            onClick={handleClear}
            name="Clear"
            className={`${currentTheme.button} w-full h-full`}
          />
        </div>
        <div className="w-18 h-12 py-1">
          <Button
            onClick={handleSave}
            name="Save"
            className={`${currentTheme.button} w-full h-full`}
          />
        </div>

        <div className="w-18 h-12 py-1">
          <Button
            onClick={handleSave}
            name="Export"
            className={`${currentTheme.button} w-full h-full`}
          />
        </div>
      </div>
    </div>
  );
};

export default MapTools;
