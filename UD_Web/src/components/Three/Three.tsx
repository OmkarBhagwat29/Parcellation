import { Grid, OrbitControls } from "@react-three/drei";
import { Canvas } from "@react-three/fiber";
import React from "react";

const Three = ({ children }) => {
  return (
    <div style={{ width: "100vw", height: "100vh" }} className="bg-slate-100">
      <Canvas style={{ width: "100%", height: "100%" }} camera={{ fov: 45 }}>
        <axesHelper />
        <OrbitControls />
        <Grid scale={5} />
        {children}
      </Canvas>
    </div>
  );
};

export default Three;
