import L from "leaflet";
import osmtogeojson from "osmtogeojson";
import { Feature, FeatureCollection } from "geojson";

export interface MapGeometry {
  buildings: Feature[];
  greenery: Feature[];
  transportation: Feature[];
  roads: Feature[];
  waterBodies: Feature[];
  landuseAreas: Feature[]; // NEW
}

export const getmapGeometry = async (
  map: L.Map
): Promise<MapGeometry | null> => {
  const bounds = map.getBounds();
  const sw = bounds.getSouthWest();
  const ne = bounds.getNorthEast();

  const query = `
    [out:json][timeout:25];
    (
      node(${sw.lat},${sw.lng},${ne.lat},${ne.lng});
      way(${sw.lat},${sw.lng},${ne.lat},${ne.lng});
      relation(${sw.lat},${sw.lng},${ne.lat},${ne.lng});
    );
    out body;
    >;
    out skel qt;
  `;

  try {
    const res = await fetch("https://overpass-api.de/api/interpreter", {
      method: "POST",
      body: query,
    });

    const data = await res.json();

    const geoJson = osmtogeojson(data, {
      flatProperties: true,
    }) as FeatureCollection;

    const features = geoJson.features;

    const isBuilding = (p: any) => p.building && p.building !== "no";

    const isGreen = (p: any) =>
      ["park", "garden", "track", "pitch"].includes(p.leisure) ||
      ["wood", "hill", "peak"].includes(p.natural) ||
      p.landuse === "cemetery";

    const isTransport = (p: any) =>
      [
        "rail",
        "light_rail",
        "subway",
        "tram",
        "monorail",
        "narrow_gauge",
        "funicular",
      ].includes(p.railway) ||
      ["platform", "stop_position"].includes(p.public_transport);

    const isRoad = (p: any) => !!p.highway;

    const isWater = (p: any) =>
      // Simple natural water bodies
      ["water", "bay", "strait"].includes(p.natural) ||
      // Waterways
      ["riverbank", "canal", "stream"].includes(p.waterway) ||
      // Landuse water storage
      ["reservoir", "basin"].includes(p.landuse) ||
      // Explicit water tagging
      ["lake", "pond", "reservoir", "basin"].includes(p.water) ||
      // Catch multipolygon water relations
      (p.type === "multipolygon" && (p.water || p.natural === "water"));

    const isLanduse = (p: any) => !!p.landuse;

    const buildings = features.filter((f) => isBuilding(f.properties));
    const greenery = features.filter((f) => isGreen(f.properties));
    const transportation = features.filter((f) => isTransport(f.properties));
    const roads = features.filter((f) => isRoad(f.properties));
    const waterBodies = features.filter((f) => isWater(f.properties));
    const landuseAreas = features.filter((f) => isLanduse(f.properties));

    return {
      buildings,
      greenery,
      transportation,
      roads,
      waterBodies,
      landuseAreas,
      all: geoJson,
    };
  } catch (error) {
    console.error("Error fetching Overpass API:", error);
    return null;
  }
};
