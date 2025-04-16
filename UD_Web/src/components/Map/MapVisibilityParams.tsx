import React, { useEffect } from "react";
import { useMap } from "./MapProvider";
import CollapsablePanel from "../Inputs/CollapsablePanel";
import { useTheme } from "@/context/ThemeContext";
import { themes } from "@/theme";
import Checkbox from "../Inputs/Checkbox";

const MapVisibilityParams = () => {
  const { geometry } = useMap();
  const { theme } = useTheme();
  const currentTheme = themes[theme];

  const [buildingVisibile, setBuildingVisible] = React.useState(true);
  const [roadVisible, setRoadVisible] = React.useState(true);
  const [waterVisible, setWaterVisible] = React.useState(true);
  const [greeneryVisible, setGreeneryVisible] = React.useState(true);
  const [tranportationVisible, setTransportationVisible] = React.useState(true);

  const [landuseVisibile, setLanduseVisible] = React.useState(true);

  const [shapesVisible, setShapesVisible] = React.useState(true);

  return (
    <>
      <CollapsablePanel
        title="Visibility Parameters"
        isCollapsed={true}
        className={`${currentTheme.collapseContainer} rounded-md relative`}
        headerClassName={`${currentTheme.collapseHeader}  p-1 h-full`}
        webviewProps={{ id: "map", command: "RESIZE" }}
      >
        <div className="grid grid-cols-2 gap-x-8 gap-y-4 p-2">
          {geometry?.buildings.length > 0 && (
            <div>
              <Checkbox
                checked={buildingVisibile}
                onChange={setBuildingVisible}
                label="Buildings"
                shape="round"
                toggleActiveClass={`${currentTheme.toggleActive}`}
                toggleInactiveClass={`${currentTheme.toggleInactive}`}
                toggleThumbClass={`${currentTheme.toggleThumb}`}
                labelClass={`${currentTheme.text}`}
                webViewProps={{
                  id: "map",
                  command: "VISIBILITY_BUILDINGS",
                }}
              />
            </div>
          )}

          {geometry?.roads.length > 0 && (
            <div>
              <Checkbox
                checked={roadVisible}
                onChange={setRoadVisible}
                label="Roads"
                shape="round"
                toggleActiveClass={`${currentTheme.toggleActive}`}
                toggleInactiveClass={`${currentTheme.toggleInactive}`}
                toggleThumbClass={`${currentTheme.toggleThumb}`}
                labelClass={`${currentTheme.text}`}
                webViewProps={{
                  id: "map",
                  command: "VISIBILITY_ROADS",
                }}
              />
            </div>
          )}

          {geometry?.greenery.length > 0 && (
            <div>
              <Checkbox
                checked={greeneryVisible}
                onChange={setGreeneryVisible}
                label="Greenery"
                shape="round"
                toggleActiveClass={`${currentTheme.toggleActive}`}
                toggleInactiveClass={`${currentTheme.toggleInactive}`}
                toggleThumbClass={`${currentTheme.toggleThumb}`}
                labelClass={`${currentTheme.text}`}
                webViewProps={{
                  id: "map",
                  command: "VISIBILITY_GREENERY",
                }}
              />
            </div>
          )}

          {geometry?.waterBodies.length > 0 && (
            <div>
              <Checkbox
                checked={waterVisible}
                onChange={setWaterVisible}
                label="Water Bodies"
                shape="round"
                toggleActiveClass={`${currentTheme.toggleActive}`}
                toggleInactiveClass={`${currentTheme.toggleInactive}`}
                toggleThumbClass={`${currentTheme.toggleThumb}`}
                labelClass={`${currentTheme.text}`}
                webViewProps={{
                  id: "map",
                  command: "VISIBILITY_WATER_BODIES",
                }}
              />
            </div>
          )}

          {geometry?.transportation.length > 0 && (
            <div>
              <Checkbox
                checked={tranportationVisible}
                onChange={setTransportationVisible}
                label="Transportation"
                shape="round"
                toggleActiveClass={`${currentTheme.toggleActive}`}
                toggleInactiveClass={`${currentTheme.toggleInactive}`}
                toggleThumbClass={`${currentTheme.toggleThumb}`}
                labelClass={`${currentTheme.text}`}
                webViewProps={{
                  id: "map",
                  command: "VISIBILITY_TRANSPORTATION",
                }}
              />
            </div>
          )}

          {geometry?.landuseAreas.length > 0 && (
            <div>
              <Checkbox
                checked={landuseVisibile}
                onChange={setLanduseVisible}
                label="Landuse"
                shape="round"
                toggleActiveClass={`${currentTheme.toggleActive}`}
                toggleInactiveClass={`${currentTheme.toggleInactive}`}
                toggleThumbClass={`${currentTheme.toggleThumb}`}
                labelClass={`${currentTheme.text}`}
                webViewProps={{
                  id: "map",
                  command: "VISIBILITY_LANDUSE",
                }}
              />
            </div>
          )}

          <div>
            <Checkbox
              checked={shapesVisible}
              onChange={setShapesVisible}
              label="Fill"
              shape="round"
              toggleActiveClass={`${currentTheme.toggleActive}`}
              toggleInactiveClass={`${currentTheme.toggleInactive}`}
              toggleThumbClass={`${currentTheme.toggleThumb}`}
              labelClass={`${currentTheme.text}`}
              webViewProps={{
                id: "map",
                command: "VISIBILITY_SHAPES",
              }}
            />
          </div>
        </div>
      </CollapsablePanel>
    </>
  );
};

export default MapVisibilityParams;
