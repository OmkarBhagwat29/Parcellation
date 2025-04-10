import { SDSessionContext } from "@/context/SDSessionContext";
import {
  createSessionDefaults,
  SDSessionManager,
} from "@/context/SDSessionManager";

import React, { useState } from "react";
import EvaluateSDSession from "./EvaluateSDSession";
import RunNewSession from "./RunNewSession";
import ExtractModelPaths from "./ExtractModelPaths";
import SDLoadModels from "./LoadModels";

const SDParcellation = () => {
  const defaults = createSessionDefaults();

  const [sd, setSdState] = useState<SDSessionManager>(defaults);

  const setSd = (update: Partial<SDSessionManager>) => {
    setSdState((prv) => ({ ...prv, ...update }));
  };

  return (
    <>
      <SDSessionContext value={{ sd, setSd }}>
        <EvaluateSDSession />
        <RunNewSession />
        <ExtractModelPaths />
        <SDLoadModels />
      </SDSessionContext>
    </>
  );
};

export default SDParcellation;
