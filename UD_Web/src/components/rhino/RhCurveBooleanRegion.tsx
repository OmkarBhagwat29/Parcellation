import React from "react";

const RhCurveBooleanRegion: React.FC = () => {
  //const [rhino, setRhino] = useState<any>(null);

  // const handleRhinoLoad = () => {
  //   if (typeof window.rhino3dm !== "undefined") {
  //     window.rhino3dm().then((rh: any) => {
  //       console.log("Rhino3dm loaded:", rh);
  //       setRhino(rh);
  //       initRhinoStuff(rh);
  //     });
  //   } else {
  //     console.error("Rhino3dm is not available on window.");
  //   }
  // };

  // const initRhinoStuff = async (rh: any) => {
  //   const circle1 = new rh.Circle([0, 0, 0], 5);
  //   const circle2 = new rh.Circle([3, 0, 0], 5);

  //   const curve1 = circle1.toNurbsCurve();
  //   const curve2 = circle2.toNurbsCurve();

  //   const curves = [curve1, curve2];
  //   //onst data = curves.map((crv: any) => JSON.parse(crv.toJSON(crv)));

  //   compute.url = "http://localhost:6500/"; // or https://compute.rhino3d.com
  //   compute.apiKey = "YOUR_API_KEY"; // if needed

  //   const plane = {
  //     Origin: { X: 0.0, Y: 0.0, Z: 0.0 },
  //     XAxis: { X: 1.0, Y: 0.0, Z: 0.0 },
  //     YAxis: { X: 0.0, Y: 1.0, Z: 0.0 },
  //     ZAxis: { X: 0.0, Y: 0.0, Z: 1.0 },
  //     Normal: { X: 0.0, Y: 0.0, Z: 1.0 },
  //   };

  //   try {
  //     console.log(compute);
  //     const result = await compute.Curve.createBooleanRegions1(
  //       curves,
  //       plane,
  //       false,
  //       0.001
  //     );
  //     console.log("Boolean Result:", result);

  //     for (const r in result) {
  //       console.log(r);
  //     }
  //   } catch (error) {
  //     console.error("Error in Compute operation:", error);
  //   }
  // };

  return (
    <>
      {/* <Script
        src="https://cdn.jsdelivr.net/npm/rhino3dm@8.17.0/rhino3dm.min.js"
        strategy="afterInteractive"
        onLoad={handleRhinoLoad}
      /> */}
      {/* <div>Check console for Rhino3dm boolean results.</div> */}
    </>
  );
};

export default RhCurveBooleanRegion;
