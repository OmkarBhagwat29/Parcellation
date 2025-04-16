import React from "react";
import Button from "../Inputs/Button";
import { useTheme } from "@/context/ThemeContext";
import { themes } from "@/theme";
import { f } from "./floodingCommand";

const SelectionParams = () => {
  const { theme } = useTheme();
  const currentTheme = themes[theme];
  return (
    <>
      <div className="flex flex-row items-center justify-center gap-2">
        <Button
          webViewProps={{
            id: "flooding",
            command: f.SELECT_TERRAIN,
          }}
          name="Select Terrain"
          className={`px-2 ${currentTheme.button}`}
        />

        <Button
          webViewProps={{
            id: "flooding",
            command: f.SELECT_OBSTACLES,
          }}
          name="Select Obstacles"
          className={`px-2 ${currentTheme.button}`}
        />
      </div>
    </>
  );
};

export default SelectionParams;
