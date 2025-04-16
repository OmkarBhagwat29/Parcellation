import React, { useState, useEffect } from "react";
import CollapsablePanel from "../Inputs/CollapsablePanel";
import Slider from "../Inputs/Slider";
import { themes } from "@/theme";
import { useTheme } from "@/context/ThemeContext";
import { f } from "./floodingCommand";

const ForceParameters = () => {
  const { theme } = useTheme();
  const currentTheme = themes[theme];

  // Initial slider values
  const [gravity, setGravity] = useState(0.098);
  const [mass, setMass] = useState(1);
  const [friction, setFriction] = useState(0.01);

  return (
    <CollapsablePanel
      title="Force Parameters"
      isCollapsed={true}
      className={`${currentTheme.collapseContainer} rounded-md`}
      headerClassName={`${currentTheme.collapseHeader}  p-1`}
      webviewProps={{ id: "flooding", command: f.UI_RESIZED }}
    >
      <div
        className={`w-full flex mt-8 px-2 transition-all duration-300 ${currentTheme}`}
      >
        <label className="min-w-[80px]">Gravity:</label>
        <div className="pl-4 w-full px-2">
          <Slider
            webViewProps={{ id: "flooding", command: f.SET_GRAVITY }}
            value={gravity}
            start={0.0}
            step={0.01}
            end={10}
            units="m/s²"
            theme={{
              track: currentTheme.sliderTrack,
              fill: currentTheme.sliderFill,
              thumb: currentTheme.sliderThumb,
              valueLabel: currentTheme.sliderValue,
            }}
            onChange={(newValue) => setGravity(newValue)} // Update gravity value
          />
        </div>
      </div>

      <div className="w-full flex mt-8 px-2">
        <label className="min-w-[80px]">Friction:</label>
        <div className="pl-4 w-full px-2">
          <Slider
            webViewProps={{
              id: "flooding",
              command: f.SET_FRICTION,
            }}
            value={friction}
            start={0.0}
            step={0.0001}
            end={1}
            units=""
            theme={{
              track: currentTheme.sliderTrack,
              fill: currentTheme.sliderFill,
              thumb: currentTheme.sliderThumb,
              valueLabel: currentTheme.sliderValue,
            }}
            onChange={(newValue) => setFriction(newValue)} // Update friction value
          />
        </div>
      </div>
    </CollapsablePanel>
  );
};

export default ForceParameters;
