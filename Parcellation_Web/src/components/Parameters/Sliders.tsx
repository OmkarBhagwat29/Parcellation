import { debounce } from "lodash";
import { useState, useCallback } from "react";
import { sendToWebView } from "../webview";

export default function RoadWidthSlider() {
  const [majorRoadWidth, setMajorRoadWidth] = useState(18);
  const [minorRoadWidth, setMinorRoadWidth] = useState(9);
  const [minParcelArea, setMinParcelArea] = useState(400);

  // Create debounced functions using useCallback
  const debouncedMajorRoadChange = useCallback(
    debounce((width) => {
      sendToWebView({
        id: "road width slider",
        command: "Major_Road_Width",
        payload: { Width: width },
      });
    }, 100),
    []
  );

  const debouncedMinorRoadChange = useCallback(
    debounce((width) => {
      sendToWebView({
        id: "road width slider",
        command: "Minor_Road_Width",
        payload: { Width: width },
      });
    }, 100),
    []
  );

  const debouncedParcelAreaChange = useCallback(
    debounce((area) => {
      sendToWebView({
        id: "parcel slider",
        command: "Min_Parcel_Area",
        payload: { Area: area },
      });
    }, 100),
    []
  );

  return (
    <div className="grid sm:grid-cols-1 select-none gap-2 relative">
      {/* Major Road Width Slider */}
      <div>
        <label className="text-sm">Major Road Width: {majorRoadWidth}m</label>
        <div className="relative w-48 mt-4">
          <input
            type="range"
            className="sm:w-48 w-48 cursor-pointer appearance-none bg-gray-300 h-1 rounded-lg outline-none"
            min={6}
            max={30}
            step={0.01}
            value={majorRoadWidth}
            onChange={(e) => {
              const width = parseFloat(e.target.value);
              setMajorRoadWidth(width);
              debouncedMajorRoadChange(width);
            }}
          />
          <div
            className="absolute text-xs bg-gray-700 text-white px-1 py-0.5 rounded"
            style={{
              left: `calc(${((majorRoadWidth - 6) / (30 - 6)) * 100}% - 5px)`,
              top: "-15px",
              whiteSpace: "nowrap",
              minWidth: "max-content",
            }}
          >
            {majorRoadWidth} m
          </div>
          <div className="flex justify-between text-sm text-gray-600">
            <span>6 m</span>
            <span>30 m</span>
          </div>
        </div>
      </div>

      {/* Minor Road Width Slider */}
      <div>
        <label className="text-sm">Minor Road Width: {minorRoadWidth}m</label>
        <div className="relative w-48 mt-4">
          <input
            type="range"
            className="sm:w-48 w-48 cursor-pointer appearance-none bg-gray-300 h-1 rounded-lg outline-none"
            min={6}
            max={18}
            step={0.01}
            value={minorRoadWidth}
            onChange={(e) => {
              const width = parseFloat(e.target.value);
              setMinorRoadWidth(width);
              debouncedMinorRoadChange(width);
            }}
          />
          <div
            className="absolute text-xs bg-gray-700 text-white px-1 py-0.5 rounded"
            style={{
              left: `calc(${((minorRoadWidth - 6) / (18 - 6)) * 100}% - 5px)`,
              top: "-15px",
              whiteSpace: "nowrap",
              minWidth: "max-content",
            }}
          >
            {minorRoadWidth} m
          </div>
          <div className="flex justify-between text-sm text-gray-600">
            <span>6 m</span>
            <span>18 m</span>
          </div>
        </div>
      </div>

      {/* Minimum Parcel Area Slider */}
      <div>
        <label className="text-sm">
          Minimum Sub Parcel Area: {minParcelArea} m²
        </label>
        <div className="relative w-48 mt-4">
          <input
            type="range"
            className="sm:w-48 w-48 cursor-pointer appearance-none bg-gray-300 h-1 rounded-lg outline-none"
            min={10}
            max={1000}
            step={1}
            value={minParcelArea}
            onChange={(e) => {
              const area = parseInt(e.target.value, 10);
              setMinParcelArea(area);
              debouncedParcelAreaChange(area);
            }}
          />
          <div
            className="absolute text-xs bg-gray-700 text-white px-1 py-0.5 rounded"
            style={{
              left: `calc(${((minParcelArea - 10) / (1000 - 10)) * 100}% - 5px)`,
              top: "-15px",
              whiteSpace: "nowrap",
              minWidth: "max-content",
            }}
          >
            {minParcelArea} m²
          </div>
          <div className="flex justify-between text-sm text-gray-600">
            <span>10 m²</span>
            <span>1000 m²</span>
          </div>
        </div>
      </div>
    </div>
  );
}
