import React, { useState } from "react";
import { useTheme } from "@/context/ThemeContext";
import { themes } from "@/theme";
import { useMap } from "../MapProvider";

const MapTabs = () => {
  const { theme } = useTheme();
  const currentTheme = themes[theme];

  const { setSelectedTab, selectedTab } = useMap();

  const onTabClick = (name: string) => {
    setSelectedTab(name);
  };

  const tabs = ["Legends", "Properties", "Projects"];

  return (
    <>
      <div className="grid grid-cols-3 gap-1 cursor-pointer">
        {tabs.map((tabName) => {
          const isSelected = selectedTab === tabName;
          return (
            <div
              key={tabName}
              onClick={() => onTabClick(tabName)}
              className={`
              flex justify-center items-center
              rounded-t-2xl py-1
              transition-all duration-200
              ${isSelected ? currentTheme.activeTab : currentTheme.button}
            `}
            >
              {tabName}
            </div>
          );
        })}
      </div>


    </>
  );
};

export default MapTabs;
