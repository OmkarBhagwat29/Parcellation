import React, { useState, useCallback } from "react";
import { debounce } from "lodash";
import { sendToWebView } from "../../../webview/webview";

const ParcelDimensions = () => {
  const [parcelDepth, setParcelDepth] = useState(60.0);
  const [parcelWidth, setLargeParcelWidth] = useState(30.0);
  const [buildingParcelDepth, setBuildingParcelDepth] = useState(20.0);
  const [buildingParcelWidth, setBuildingParcelWidth] = useState(10.0);

  // Memoized debounced function
  const sendParcelUpdate = useCallback(
    debounce((depth, width) => {
      sendToWebView({
        id: "parcel",
        command: "SUB_PARCEL_SIZES",
        payload: { ParcelDepth: depth, ParcelWidth: width },
      });
    }, 300),
    []
  );

  const sendBuildingParcelUpdate = useCallback(
    debounce((depth, width) => {
      sendToWebView({
        id: "building",
        command: "BUILDING_PARCEL_SIZES",
        payload: { ParcelDepth: depth, ParcelWidth: width },
      });
    }, 300),
    []
  );

  return (
    <div className="grid grid-cols-2 text-sm gap-4">
      <div className="flex flex-col gap-4">
        {/* Large Parcel Depth */}
        <div>
          <label>Sub Parcel Depth:</label>
          <div className="flex items-center gap-x-1 text-sm mt-1">
            <input
              type="number"
              min={0}
              max={10000}
              value={parcelDepth}
              onChange={(e) => {
                const depth = parseFloat(e.target.value) || 60;
                setParcelDepth(depth);
                sendParcelUpdate(depth, parcelWidth);
              }}
              className="border p-1 w-20"
            />
            <span>mt</span>
          </div>
        </div>

        {/* Large Parcel Width */}
        <div>
          <label>Sub Parcel Width:</label>
          <div className="flex items-center gap-x-1 mt-1 text-sm">
            <input
              type="number"
              min={0}
              max={10000}
              value={parcelWidth}
              onChange={(e) => {
                const width = parseFloat(e.target.value) || 100;
                setLargeParcelWidth(width);
                sendParcelUpdate(parcelDepth, width);
              }}
              className="border p-1 w-20"
            />
            <span>mt</span>
          </div>
        </div>
      </div>

      <div className="flex flex-col gap-4">
        {/* Small Parcel Depth */}
        <div>
          <label>Building Parcel Depth:</label>
          <div className="flex items-center gap-x-1 text-sm mt-1">
            <input
              type="number"
              min={0}
              max={10000}
              value={buildingParcelDepth}
              onChange={(e) => {
                const depth = parseFloat(e.target.value) || 0;
                setBuildingParcelDepth(depth);
                sendBuildingParcelUpdate(depth, buildingParcelWidth);
              }}
              className="border p-1 w-20"
            />
            <span>mt</span>
          </div>
        </div>

        {/* Small Parcel Width */}
        <div>
          <label>Building Parcel Width:</label>
          <div className="flex items-center gap-x-1 text-sm mt-1">
            <input
              type="number"
              min={0}
              max={10000}
              value={buildingParcelWidth}
              onChange={(e) => {
                const width = parseFloat(e.target.value) || 0;
                setBuildingParcelWidth(width);
                sendBuildingParcelUpdate(buildingParcelDepth, width);
              }}
              className="border p-1 w-20"
            />
            <span>mt</span>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ParcelDimensions;
