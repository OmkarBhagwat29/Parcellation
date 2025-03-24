import React from "react";
import AI_Chat from "./AI/AI_Chat";
import Charts from "./Charts";

const Analytics = () => {
  return (
    <div
      id="analytics-panel"
      className="h-[825px] w-[350px] rounded-lg flex flex-col"
    >
      <div className="p-2">
        {/* <label>Charts</label> */}
        <Charts />
      </div>

      <div className="pl-2 pr-2 flex-1 flex flex-col">
        <label className=" text-sm">Ai Assistant</label>

        <AI_Chat />
      </div>
    </div>
  );
};

export default Analytics;
