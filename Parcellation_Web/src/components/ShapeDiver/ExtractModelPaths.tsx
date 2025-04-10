import { useSDSessionContext } from "@/context/SDSessionContext";
import { Model_Name_Paths } from "@/context/SDSessionManager";
import { ShapeDiverResponseOutput } from "@shapediver/sdk.geometry-api-sdk-v2";
import React, { useEffect } from "react";

const ExtractModelPaths = () => {
  const { sd, setSd } = useSDSessionContext();

  useEffect(() => {
    if (!sd.newState) return;

    const getModelPaths = () => {
      const data: Model_Name_Paths[] = [];
      for (const o in sd.newState?.outputs) {
        const outputVersion = sd.newState.outputs[
          o
        ] as ShapeDiverResponseOutput;

        const prevOutputVersion = sd.defaultState!.outputs![
          o
        ] as ShapeDiverResponseOutput;

        if (outputVersion.version !== prevOutputVersion.version) {
          const content = outputVersion.content;

          if (!content) return;

          const paths = [];
          for (let i = 0; i < content.length; i++) {
            const c = content[i];
            console.log(c);
            if (c.href) {
              paths.push(c.href);
            }
          }

          if (paths.length > 0) {
            data.push({ name: outputVersion.name, paths });
          }
        }
      }

      if (data.length > 0) {
        setSd({ modelData: data });
        console.log(data);
      }
    };

    getModelPaths();
  }, [sd.newState]);

  // const models = useGLTF(modelPaths);
  // console.log(models);
  return (
    <>
      {/* {models.map((m) => (
        <primitive key={m.scene.id} object={m.scene} />
      ))} */}
    </>
  );
};

export default ExtractModelPaths;
