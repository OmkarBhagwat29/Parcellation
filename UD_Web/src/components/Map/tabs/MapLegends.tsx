import React, { useEffect } from "react";
import { useMap } from "../MapProvider";
import { useTheme } from "@/context/ThemeContext";
import { themes } from "@/theme";

// Color mappings (from earlier C# logic)
const colorMap: Record<string, string> = {
  residential: "#FFECB3",
  apartments: "#FFECB3",
  house: "#FFECB3",
  detached: "#FFECB3",
  dormitory: "#FFECB3",
  terrace: "#FFECB3",
  hotel: "#FFCC99",
  hostel: "#FFCC99",
  commercial: "#FFDF00",
  retail: "#FFDF00",
  shop: "#FFDF00",
  mall: "#FFDF00",
  supermarket: "#FFDF00",
  kiosk: "#FFDF00",
  marketplace: "#FFDF00",
  office: "#0099CC",
  warehouse: "#BF9050",
  factory: "#BF9050",
  silo: "#BF9050",
  garage: "#BF9050",
  carport: "#BF9050",
  hangar: "#BF9050",
  digester: "#BF9050",
  boilerhouse: "#BF9050",
  storagetank: "#BF9050",
  school: "#AADE77",
  university: "#AADE77",
  kindergarten: "#AADE77",
  hospital: "#FF8080",
  clinic: "#FF8080",
  church: "#CA99FF",
  cathedral: "#CA99FF",
  mosque: "#CA99FF",
  synagogue: "#CA99FF",
  temple: "#CA99FF",
  shrine: "#CA99FF",
  monastery: "#CA99FF",
  placeofworship: "#CA99FF",
  religious: "#CA99FF",
  chapel: "#CA99FF",
  public: "#99CCFF",
  civic: "#99CCFF",
  government: "#99CCFF",
  courthouse: "#99CCFF",
  prison: "#99CCFF",
  police: "#99CCFF",
  firestation: "#99CCFF",
  embassy: "#99CCFF",
  customs: "#99CCFF",
  communitycentre: "#99CCFF",
  sportsfacility: "#9966FF",
  stadium: "#9966FF",
  gym: "#9966FF",
  sportshall: "#9966FF",
  construction: "#B4B4B4",
  underconstruction: "#B4B4B4",
  proposed: "#B4B4B4",
  abandoned: "#B4B4B4",
  disused: "#B4B4B4",
  historic: "#A88664",
  castle: "#A88664",
  fortress: "#A88664",
  fort: "#A88664",
  ruin: "#A88664",
  trainstation: "#72AEBE",
  transportation: "#72AEBE",
  busstation: "#72AEBE",
  ferryterminal: "#72AEBE",
  airportterminal: "#72AEBE",
  controltower: "#72AEBE",
  tollbooth: "#72AEBE",
  gatehouse: "#72AEBE",
  guardhouse: "#72AEBE",
  powerstation: "#A0A0D2",
  transformertower: "#A0A0D2",
  substation: "#A0A0D2",
  watertower: "#A0A0D2",
  windmill: "#A0A0D2",
  lighthouse: "#A0A0D2",
  bridge: "#A0A0D2",
  tower: "#A0A0D2",
  farm: "#CBE9A1",
  barn: "#CBE9A1",
  stable: "#CBE9A1",
  greenhouse: "#CBE9A1",
  shed: "#CBE9A1",
  cabin: "#CBE9A1",
  hut: "#CBE9A1",
  service: "#C8C8A0",
  shelter: "#C8C8A0",
  toilet: "#C8C8A0",
  container: "#C8C8A0",
  tent: "#C8C8A0",
  roof: "#C8C8A0",
  bunker: "#C8C8A0",
  pavilion: "#C8C8A0",
  military: "#9B6464",
  theatre: "#FFCCE5",
  cinema: "#FFCCE5",
  museum: "#FFCCE5",
  library: "#FFCCE5",
  zoo: "#FFCCE5",
  aquarium: "#FFCCE5",
  planetarium: "#FFCCE5",
  observatory: "#FFCCE5",
  parking: "#BEBEFF",
  forest: "#AADE77",
  grass: "#AADE77",
  meadow: "#AADE77",
  orchard: "#AADE77",
  vineyard: "#AADE77",
  cemeterygreen: "#AADE77",
  farmland: "#CBE9A1",
  farmyard: "#CBE9A1",
  allotments: "#CBE9A1",
  cemetery: "#CCCCFF",
  recreationground: "#90EE90",
  quarry: "#BDB76B",
  landfill: "#BDB76B",
  greenfield: "#B4B4B4",
  brownfield: "#B4B4B4",
  basin: "#ADD8E6",
  reservoir: "#ADD8E6",
  water: "#ADD8E6",
  waterway: "#ADD8E6",
  river: "#ADD8E6",
};

const MapLegends = () => {
  const { geometry, appDimensions } = useMap();
  const { theme } = useTheme();
  const currentTheme = themes[theme];

  useEffect(() => {
    // sendToWebView({ id: "map", command: "RESIZE" });
  }, []);

  const fallbackColor = (mainType: string, subtype: string) =>
    `hsl(${((mainType + subtype).length * 37) % 360}, 60%, 60%)`;

  const renderLegendItem = (label: string, color: string) => (
    <div key={label} className="flex items-center space-x-2">
      <div
        className="w-4 h-4 rounded-full"
        style={{ backgroundColor: color }}
      />
      <span className="text-sm capitalize">{label}</span>
    </div>
  );

  const subtypeKeys: Record<string, string[]> = {
    buildings: ["building"],
    greenery: ["leisure", "natural", "landuse"],
    transportation: ["railway", "public_transport"],
    roads: ["highway"],
    waterBodies: ["natural", "waterway", "water", "landuse", "type"],
    landuseAreas: ["landuse"],
  };

  if (!geometry) return null;

  return (
    <div
      className={`w-full space-y-4 ${currentTheme.background} ${currentTheme.text} overflow-auto`}
      style={{ height: `${appDimensions.height}` }}
    >
      <h2 className="text-xl font-semibold">Map Legend</h2>

      {Object.entries(geometry).map(([mainType, features]) => {
        if (!features || features.length === 0) return null;

        const subtypeMap: Record<string, number> = {};

        features.forEach((feature) => {
          const props = feature.properties;
          subtypeKeys[mainType]?.forEach((key) => {
            const value = props?.[key];
            if (value) {
              const valStr = String(value).toLowerCase();
              if (valStr !== "yes") {
                subtypeMap[valStr] = (subtypeMap[valStr] || 0) + 1;
              }
            }
          });
        });

        const subtypes = Object.keys(subtypeMap);

        return (
          <div key={mainType}>
            <h3 className="text-md font-medium capitalize mb-2">{mainType}</h3>
            <div className="space-y-1 ml-2">
              {subtypes.map((subtype) => {
                return renderLegendItem(
                  subtype,
                  colorMap[subtype] || fallbackColor(mainType, subtype)
                );
              })}
            </div>
          </div>
        );
      })}
    </div>
  );
};

export default MapLegends;
