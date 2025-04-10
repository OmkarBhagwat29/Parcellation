import { useParamsContext } from "@/context/ParcellationParamsContext";
import { useSDSessionContext } from "@/context/SDSessionContext";
import { useThree } from "@react-three/fiber";
import React, { useEffect } from "react";

const RunNewSession = () => {
  const { pm } = useParamsContext();
  const { sd, setSd } = useSDSessionContext();
  const { scene } = useThree();

  useEffect(() => {
    if (!pm || !sd.defaultState) return;

    const customizeSession = async () => {
      const params = {
        Radius: Math.round(pm.majorRoadWidth),
      };

      const newState = await sd.sdk?.utils.submitAndWaitForCustomization(
        sd.sdk!,
        sd.defaultState!.sessionId!,
        params
      );

      if (newState && sd.modelIds.length > 0) {
        console.log("removing old object");
        sd.modelIds.forEach((obj) => scene.remove(obj));
      }

      console.log(params);
      console.log("New Evaluated State:", newState);
      setSd({ newState, modelIds: [] });
    };

    customizeSession();
  }, [pm, sd.defaultState]);
  return <></>;
};

export default RunNewSession;
