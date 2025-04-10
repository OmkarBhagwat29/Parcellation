import React from "react";
import Checkbox from "../Inputs/Checkbox";
import Button from "../Inputs/Button";
import Slider from "../Inputs/Slider";
import { useTheme } from "@/context/ThemeContext";
import { themes } from "@/theme";
import SlideableToggle from "../Inputs/SlideableToggle";

const FloodingToolUi = () => {
  const { theme } = useTheme();

  const currentTheme = themes[theme];

  console.log(currentTheme);
  return (
    <>
      <div
        className={`m-2 p-2 w-50 flex flex-col items-center justify-center select-none ${currentTheme.background} gap-4`}
      >
        <Button
          webViewProps={{ id: "flooding", command: "SELECT_TERRAIN" }}
          name="Select Terrain"
          width="w-auto"
          maxWidth="150px"
        />

        <Button
          webViewProps={{ id: "flooding", command: "SELECT_OBSTACLES" }}
          name="Select Obstacles"
          width="w-auto"
          maxWidth="150px"
        />

        <div className="flex flex-row items-center justify-center gap-2">
          <label>Run:</label>
          <div className="w-16 px-1">
            <SlideableToggle
              currentValue={true}
              webViewProps={{ id: "flooding", command: "RUN_SIMULATION" }}
            />
          </div>
        </div>

        <Button
          webViewProps={{ id: "flooding", command: "RESET_SIMULATION" }}
          name="Reset"
          width="w-20"
        />

        <div className="w-full flex mt-10">
          <label>Gravity:</label>
          <div className="pl-4 w-full">
            <Slider
              webViewProps={{ id: "flooding", command: "SET_GRAVITY" }}
              value={1}
              start={0.0}
              step={0.0001}
              end={1}
              units="m/s2"
            />
          </div>
        </div>
      </div>
    </>
  );
};

export default FloodingToolUi;
