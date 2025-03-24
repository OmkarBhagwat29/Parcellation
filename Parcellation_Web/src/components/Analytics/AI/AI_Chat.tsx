import React, { useState } from "react";
import ResponseFromAi from "./Response";
import QueryToAi from "./Query";

const AI_Chat = () => {
  const [loading, setLoading] = useState(false); // Track loading state

  // Function to set loading state
  const handleLoadingState = (isLoading) => {
    setLoading(isLoading);
  };

  return (
    <div className="w-full h-full flex flex-1 flex-col">
      <ResponseFromAi loading={loading} setLoading={setLoading} />
      <QueryToAi loading={loading} handleLoadingState={handleLoadingState} />
    </div>
  );
};

export default AI_Chat;
