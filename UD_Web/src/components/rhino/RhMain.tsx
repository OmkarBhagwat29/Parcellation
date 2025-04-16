"use client";
import {
  createDefaultParcellationParams,
  ParcellationParameters,
} from "@/context/ParcellationParameters";
import { ParamsContext } from "@/context/ParcellationParamsContext";
import React, { useState } from "react";
import Parcellation from "../Parcellation/Parcellation";
import ThreeMain from "../Three/ThreeMain";
import { Box } from "@react-three/drei";
import RhCurveBooleanRegion from "./RhCurveBooleanRegion";

const RhMain = () => {
  const defaults = createDefaultParcellationParams();

  const [pm, setPmState] = useState<ParcellationParameters>(defaults);

  // ✅ Merge previous state with new updates
  const setPm = (update: Partial<ParcellationParameters>) => {
    setPmState((prev) => ({ ...prev, ...update }));
  };

  return (
    <div className="flex w-screen h-screen overflow-hidden">
      <ParamsContext value={{ pm: pm, setPm }}>
        {" "}
        <RhCurveBooleanRegion />
        <Parcellation isWebView={false} />
        <ThreeMain>
          <Box />
        </ThreeMain>
      </ParamsContext>
    </div>
  );
};

export default RhMain;
