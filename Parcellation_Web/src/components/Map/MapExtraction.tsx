import React from "react";
import { useMap } from "./MapProvider";

const MapExtraction = () => {
  const { isExtracting } = useMap();

  return <>{isExtracting && <div>Extraction is in Progress!!!</div>}</>;
};

export default MapExtraction;
