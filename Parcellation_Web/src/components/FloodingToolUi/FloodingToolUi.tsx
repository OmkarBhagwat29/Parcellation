import React, { useEffect, useState } from "react";
import { useTheme } from "@/context/ThemeContext";
import { themes } from "@/theme";
import ForceParameters from "./ForceParams";
import SelectionParams from "./SelectionParams";
import PlayResetParams from "./PlayResetParams";
import { isWebViewMode } from "@/webview/webview";
import ParticleParams from "./ParticleParams";
import VisibilityParams from "./VisibilityParams";

const FloodingToolUi = () => {
  const { theme, setTheme } = useTheme();

  const currentTheme = themes[theme];

  const [contextMenu, setContextMenu] = useState<{
    x: number;
    y: number;
    visible: boolean;
  }>({ x: 0, y: 0, visible: false });

  const [webViewAvailable, setWebViewAvailable] = useState(false);

  useEffect(() => {
    setWebViewAvailable(isWebViewMode());
  }, []);

  // Send to webview after right-click context menu is shown
  const handleRightClick = (e: React.MouseEvent) => {
    e.preventDefault();
    setContextMenu({ x: e.clientX, y: e.clientY, visible: true });
  };

  // Send to webview when a theme is selected
  const handleThemeSelect = (selectedTheme: keyof typeof themes) => {
    setTheme(selectedTheme);
    setContextMenu({ ...contextMenu, visible: false });
  };

  // Close context menu and send info to webview after closing
  const handleClickAnywhere = () => {
    if (contextMenu.visible) {
      setContextMenu({ ...contextMenu, visible: false });
    }
  };

  return (
    <>
      <div
        id="main"
        onContextMenu={handleRightClick}
        onClick={handleClickAnywhere}
        className={`p-2 w-64 flex flex-col select-none ${currentTheme.background} gap-4 max-h-[750px] overflow-auto`}
      >
        <SelectionParams />
        <PlayResetParams />
        <ForceParameters />
        <ParticleParams />
        <VisibilityParams />
      </div>

      {contextMenu.visible && (
        <div
          id="theme-menu"
          style={{
            position: "fixed",
            top: webViewAvailable ? 0 : contextMenu.y, // Top-left when webview is available, else follow mouse
            left: webViewAvailable ? 0 : contextMenu.x, // Same as above
            backgroundColor: "#fff",
            border: "1px solid #ccc",
            borderRadius: "0.25rem",
            padding: "0.5rem",
            zIndex: 1000,
          }}
          onClick={(e) => e.stopPropagation()}
        >
          {Object.keys(themes).map((themeName) => (
            <div
              key={themeName}
              onClick={() =>
                handleThemeSelect(themeName as keyof typeof themes)
              }
              className="cursor-pointer px-2 py-1 hover:bg-gray-200"
            >
              {themeName}
            </div>
          ))}
        </div>
      )}
    </>
  );
};

export default FloodingToolUi;
