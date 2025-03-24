"use client";

import React from "react";
import Analytics from "./Analytics/Analytics";
import Parameters from "./Parameters/Parameters";

const Parecelization = () => {
  return (
    <div
      id="main"
      className="flex flex-row gap-4 h-[860px] w-[720px] p-2 pl-7 select-none"
    >
      {/* Sidebar Parameters */}
      <Parameters />

      {/* Analytics Panel */}

      <Analytics />
    </div>
  );
};

export default Parecelization;
