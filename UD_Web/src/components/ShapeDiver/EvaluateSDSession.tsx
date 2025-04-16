import { useParamsContext } from "@/context/ParcellationParamsContext";
import { useSDSessionContext } from "@/context/SDSessionContext";
import { create, ShapeDiverSdk } from "@shapediver/sdk.geometry-api-sdk-v2";
import React, { useEffect, useRef } from "react";

let sdkInstance: ShapeDiverSdk | null = null;

export const getShapeDiverSdk = (): ShapeDiverSdk => {
  if (!sdkInstance) {
    sdkInstance = create(process.env.NEXT_PUBLIC_SD_MODEL_URL);
  }
  return sdkInstance;
};

const EvaluateSDSession = () => {
  const { pm } = useParamsContext();
  const { sd, setSd } = useSDSessionContext();
  const hasInitialized = useRef(false); // Prevent multiple calls

  useEffect(() => {
    //return;
    if (hasInitialized.current) return; // Prevent multiple initializations
    hasInitialized.current = true;

    const SetShapeDiverSDK = async () => {
      try {
        const sdk = getShapeDiverSdk();

        console.log("Initializing SDK...");
        const state = await sdk.session.init(
          process.env.NEXT_PUBLIC_SD_API_TICKET
        );

        setSd({ sdk, defaultState: state,  });

        console.log("SDK Initialized:", sdk);
        console.log("default state:", state);
      } catch (err) {
        console.error("Error initializing SDK:", err);
      }
    };

    SetShapeDiverSDK();
  }, []);

  return null;
};

export default EvaluateSDSession;
