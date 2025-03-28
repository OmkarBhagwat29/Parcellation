import React from "react";
import { sendToWebView } from "../../app/webview/webview";

const Selection = () => {
  const sendMessageToRhino = (
    id: string,
    command: string,
    payload?: string
  ) => {
    if (window && window.chrome && window.chrome.webview) {
      //window.postMessage({ data: message }, "*");

      window.chrome.webview.postMessage({ id, command, payload });
    }
  };

  return (
    <div className="grid grid-cols-2 sm:gap-1 gap-2 ">
      <div className="flex flex-col items-center justify-center gap-2">
        <button
          id="selectParcel"
          className="bg-slate-300 rounded-md min-h-[35px] shadow-md px-1 py-1 hover:bg-slate-200 transition-colors duration-100 active:bg-slate-50 text-sm"
          onClick={(e: React.MouseEvent) =>
            // sendMessageToRhino("parcel", "Select_Parcel")
            sendToWebView({ id: "parcel", command: "Select_Parcel" })
          }
        >
          Select Parcel
        </button>
        <button
          id="selectRoads"
          className="bg-slate-300 transition-colors duration-100 px-1 text-sm hover:bg-slate-200 rounded-md min-h-[35px] shadow-md py-1 active:bg-slate-50"
          onClick={(e: React.MouseEvent) =>
            sendToWebView({
              id: "road network",
              command: "Select_Road_Network",
            })
          }
        >
          Select Road Network
        </button>
      </div>

      <div className="flex flex-col items-center justify-center gap-2">
        <button
          className="bg-slate-300 rounded-md min-h-[35px] shadow-md px-1 py-1 hover:bg-slate-200 transition-colors duration-100 active:bg-slate-50 text-sm"
          onClick={(e: React.MouseEvent) =>
            sendToWebView({ id: "attractor", command: "CITY_ATTRACTOR" })
          }
        >
          Select City Attractor
        </button>
        <button
          id="selectRoads"
          className="bg-slate-300 transition-colors duration-100 px-1 text-sm hover:bg-slate-200 rounded-md min-h-[35px] shadow-md py-1 active:bg-slate-50"
          onClick={(e: React.MouseEvent) =>
            sendToWebView({ id: "road network", command: "CITY_GREEN_POINTS" })
          }
        >
          Select Green Points
        </button>
      </div>
    </div>
  );
};

export default Selection;
