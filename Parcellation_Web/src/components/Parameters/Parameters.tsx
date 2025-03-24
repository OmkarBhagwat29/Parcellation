import React from "react";
import Selection from "./Selection";
import Sliders from "./Sliders";
import ParcelDimentsions from "./ParcelDimentsions";
import RangeSlider from "./RangeSlider";
import Visibility from "../Visibility/Visibility";
import InfoDisplay from "./InfoDisplay";

const Parameters = () => {
  return (
    <div id="params" className="bg-whiteshadow-lg rounded-lg w-[300px]">
      <label className="text-lg text-slate-600">Parcellation</label>

      <div className="mt-2">
        <Selection />
      </div>

      <div className="mt-4">
        <Sliders />
      </div>

      <div className="mt-4">
        <ParcelDimentsions />
      </div>

      <div>
        <RangeSlider />
      </div>

      <div className="mt-4">
        <Visibility />
      </div>

      <div className="mt-4">
        <InfoDisplay />
      </div>
    </div>
  );
};

export default Parameters;
