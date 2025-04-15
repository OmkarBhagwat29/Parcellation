import React, { useState } from "react";
import Checkbox from "../Inputs/Checkbox";
import { themes } from "@/theme";
import { useTheme } from "@/context/ThemeContext";
import { f } from "./floodingCommand";

const VisibilityParams = () => {
  const [pointChecked, setPointChecked] = useState(true);
  const [plChecked, setPlChecked] = useState(true);

  const { theme } = useTheme();
  const currentTheme = themes[theme];

  return (
    <>
      <div className="flex flex-col gap-2">
        <label className={`px-1 ${currentTheme.text}`}>
          Visibility Parameters
        </label>
        <div className="px-2 flex flex-row items-center justify-between">
          <div>
            <Checkbox
              checked={pointChecked}
              onChange={setPointChecked}
              label="Points"
              shape="round"
              toggleActiveClass={`${currentTheme.toggleActive}`}
              toggleInactiveClass={`${currentTheme.toggleInactive}`}
              toggleThumbClass={`${currentTheme.toggleThumb}`}
              labelClass={`${currentTheme.text}`}
              webViewProps={{
                id: "flooding",
                command: f.VISIBILITY_POINTS,
              }}
            />
          </div>

          <div>
            <Checkbox
              checked={plChecked}
              onChange={setPlChecked}
              label="Paths"
              shape="round"
              toggleActiveClass={`${currentTheme.toggleActive}`}
              toggleInactiveClass={`${currentTheme.toggleInactive}`}
              toggleThumbClass={`${currentTheme.toggleThumb}`}
              labelClass={`${currentTheme.text}`}
              webViewProps={{
                id: "flooding",
                command: f.VISIBILITY_PATHS,
              }}
            />
          </div>
        </div>
      </div>
    </>
  );
};

export default VisibilityParams;
