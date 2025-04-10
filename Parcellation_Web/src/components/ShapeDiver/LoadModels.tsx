import { useSDSessionContext } from "@/context/SDSessionContext";
import { useThree } from "@react-three/fiber";
import React, { useEffect } from "react";
import { Mesh, MeshStandardMaterial } from "three";
import { GLTFLoader } from "three/examples/jsm/Addons.js";

const SDLoadModels = () => {
  const { sd, setSd } = useSDSessionContext();
  const { scene } = useThree();
  //   const models = useGLTF(sd.modelPaths);

  useEffect(() => {
    if (sd.modelData.length == 0) return;

    const loadModels = async () => {
      const gltfLoader = new GLTFLoader();
      const gltf = await gltfLoader.loadAsync(sd.modelData[0].paths[0]);

      console.log(gltf);

      if (sd.modelData[0].name === "glTFDisplay") {
        gltf.scene.traverse((child) => {
          if (child instanceof Mesh) {
            child.material = new MeshStandardMaterial({
              color: "blue",
              wireframe: true,
            });
          }
        });

        scene.add(gltf.scene);
        setSd({ modelIds: [gltf.scene] });
      }

      console.log(scene.children);
    };

    loadModels();
  }, [sd.modelData]);

  return <></>;
};

export default SDLoadModels;
