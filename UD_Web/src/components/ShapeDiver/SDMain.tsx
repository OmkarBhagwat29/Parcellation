"use client";
import React, { useState } from "react";
import Parcellation from "../Parcellation/Parcellation";
import ThreeMain from "../Three/ThreeMain";
import SDParcellation from "./SDParcellation";
import { ParamsContext } from "@/context/ParcellationParamsContext";
import {
  createDefaultParcellationParams,
  ParcellationParameters,
} from "@/context/ParcellationParameters";

const SDMain = () => {
  const defaults = createDefaultParcellationParams();

  const [pm, setPmState] = useState<ParcellationParameters>(defaults);

  // ✅ Merge previous state with new updates
  const setPm = (update: Partial<ParcellationParameters>) => {
    setPmState((prev) => ({ ...prev, ...update }));
  };

  return (
    <>
      <div className="flex w-screen h-screen overflow-hidden">
        <ParamsContext value={{ pm: pm, setPm }}>
          <Parcellation isWebView={false} />

          <ThreeMain>
            <SDParcellation />
            <ambientLight />
          </ThreeMain>
        </ParamsContext>
      </div>
    </>
  );
};

export default SDMain;
