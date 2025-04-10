import React from "react";
import Selection from "../Inputs/Parameters/Selection";
import Sliders from "../Inputs/Parameters/Sliders";
import ParcelDimentsions from "../Inputs/Parameters/ParcelDimentsions";
import RangeSlider from "../Inputs/Parameters/RangeSlider";
import InfoDisplay from "../Inputs/Parameters/InfoDisplay";
import Visibility from "../Inputs/Visibility/Visibility";

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
