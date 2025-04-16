import React, { useState } from "react";
import ResponseFromAi from "./Response";
import QueryToAi from "./Query";

const AI_Chat = () => {
  const [loading, setLoading] = useState(false); // Track loading state

  // Function to set loading state
  const handleLoadingState = (isLoading: boolean) => {
    setLoading(isLoading);
  };

  return (
    <div className="w-full h-full flex flex-col flex-1 min-h-0 overflow-hidden">
      <div className="flex-1 min-h-0">
        <ResponseFromAi loading={loading} setLoading={setLoading} />
      </div>
      <div className="flex-none">
        <QueryToAi loading={loading} handleLoadingState={handleLoadingState} />
      </div>
    </div>
  );
};

export default AI_Chat;
