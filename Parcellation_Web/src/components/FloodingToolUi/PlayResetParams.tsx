import { useTheme } from "@/context/ThemeContext";
import { themes } from "@/theme";
import React, { useState } from "react";
import Button from "../Inputs/Button";
import SlideableToggle from "../Inputs/SlideableToggle";
import { f } from "./floodingCommand";
import Slider from "../Inputs/Slider";

const PlayResetParams = () => {
  const { theme } = useTheme();
  const currentTheme = themes[theme];

  const [tiemStep, setTimeStep] = useState(4.25);

  return (
    <div className="flex flex-col">
      <div className=" flex flex-row items-center justify-center gap-6">
        <div className="">
          <Button
            webViewProps={{
              id: "flooding",
              command: f.RESET_SIMULATION,
            }}
            name="Reset"
            className={`px-2 py-1 ${currentTheme.button}`}
          />
        </div>

        <div className="flex flex-row items-center justify-center gap-2">
          <label>Run:</label>
          <div className="w-16 px-1">
            <SlideableToggle
              currentValue={true}
              webViewProps={{
                id: "flooding",
                command: f.RUN_SIMULATION,
              }}
              theme={{
                activeBg: currentTheme.toggleActive,
                inactiveBg: currentTheme.toggleInactive,
                thumb: currentTheme.toggleThumb,
              }}
            />
          </div>
        </div>
      </div>

      <div className="w-full mt-10 flex flex-row px-1">
        <label className={`min-w-[80px] ${currentTheme.text}`}>
          Time Step:
        </label>
        <div className="w-full px-2">
          <Slider
            webViewProps={{
              id: "flooding",
              command: f.SET_TIME_STEP,
            }}
            value={tiemStep}
            start={0.1}
            step={0.001}
            end={10}
            units=""
            theme={{
              track: currentTheme.sliderTrack,
              fill: currentTheme.sliderFill,
              thumb: currentTheme.sliderThumb,
              valueLabel: currentTheme.sliderValue,
            }}
            onChange={(newValue) => setTimeStep(newValue)} // Update mass value
          />
        </div>
      </div>
    </div>
  );
};

export default PlayResetParams;
