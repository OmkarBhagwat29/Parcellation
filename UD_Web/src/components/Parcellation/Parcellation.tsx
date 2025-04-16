import React from "react";
import Analytics from "../Analytics/Analytics";
import Parameters from "./Parameters";

const Parcellation = ({ isWebView }) => {
  // const [isWebView, setIsWebView] = useState(true);

  // useEffect(() => {
  //   setIsWebView(isWebViewMode());
  // }, []);

  return (
    <div
      id="main"
      className={`flex flex-row gap-4 h-[850px]  p-2 select-none overflow-hidden ${
        isWebView
          ? "w-[720px] min-w-[720px] max-w-[720px] pl-7 "
          : "w-[320px] min-w-[320px] max-w-[320px]"
      }`}
    >
      {/* Sidebar Parameters */}
      <Parameters />

      {/* Analytics Panel */}

      {isWebView && <Analytics />}
    </div>
  );
};

export default Parcellation;
