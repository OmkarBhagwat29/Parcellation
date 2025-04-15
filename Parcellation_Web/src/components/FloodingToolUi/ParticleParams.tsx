import React, { useState } from "react";
import CollapsablePanel from "../Inputs/CollapsablePanel";
import { useTheme } from "@/context/ThemeContext";
import { themes } from "@/theme";
import { f } from "./floodingCommand";
import Slider from "../Inputs/Slider";

const ParticleParams = () => {
  const { theme } = useTheme();
  const currentTheme = themes[theme];

  // Initial slider values
  const [radius, setRadius] = useState(2);
  const [mass, setMass] = useState(1);

  return (
    <CollapsablePanel
      title="Particle Parameters"
      isCollapsed={true}
      className={`${currentTheme.collapseContainer} rounded-md`}
      headerClassName={`${currentTheme.collapseHeader}  p-1`}
      webviewProps={{ id: "flooding", command: f.UI_RESIZED }}
    >
      <div
        className={`w-full flex mt-8 px-2 transition-all duration-300 ${currentTheme}`}
      >
        <label className="min-w-[80px]">Radius:</label>
        <div className="pl-4 w-full px-2">
          <Slider
            webViewProps={{ id: "flooding", command: f.SET_PARTICLE_RADIUS }}
            value={radius}
            start={0.0}
            step={0.001}
            end={5}
            units="m"
            theme={{
              track: currentTheme.sliderTrack,
              fill: currentTheme.sliderFill,
              thumb: currentTheme.sliderThumb,
              valueLabel: currentTheme.sliderValue,
            }}
            onChange={(newValue) => setRadius(newValue)} // Update gravity value
          />
        </div>
      </div>

      <div className="w-full flex mt-8 px-2">
        <label className="min-w-[80px]">Mass:</label>
        <div className="pl-4 w-full px-2">
          <Slider
            webViewProps={{
              id: "flooding",
              command: f.SET_MASS,
            }}
            value={mass}
            start={0.001}
            step={0.001}
            end={10}
            units="gm"
            theme={{
              track: currentTheme.sliderTrack,
              fill: currentTheme.sliderFill,
              thumb: currentTheme.sliderThumb,
              valueLabel: currentTheme.sliderValue,
            }}
            onChange={(newValue) => setMass(newValue)} // Update mass value
          />
        </div>
      </div>
    </CollapsablePanel>
  );
};

export default ParticleParams;
