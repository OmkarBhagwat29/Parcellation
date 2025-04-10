import React from "react";
import { sendToWebView } from "../../../webview/webview";
import { useParamsContext } from "@/context/ParcellationParamsContext";

const Selection = () => {
  const { pm, setPm } = useParamsContext();

  return (
    <div className="grid grid-cols-2 sm:gap-1 gap-2 ">
      <div className="flex flex-col items-center justify-center gap-2">
        <button
          id="selectParcel"
          className="bg-slate-300 rounded-md min-h-[35px] shadow-md px-1 py-1 hover:bg-slate-200 transition-colors duration-100 active:bg-slate-50 text-sm"
          onClick={(e: React.MouseEvent) => {
            // sendMessageToRhino("parcel", "Select_Parcel")
            sendToWebView({ id: "parcel", command: "Select_Parcel" });

            setPm({ parcelPoints: [[2, 2, 2]] });
          }}
        >
          Select Parcel
        </button>
        <button
          id="selectRoads"
          className="bg-slate-300 transition-colors duration-100 px-1 text-sm hover:bg-slate-200 rounded-md min-h-[35px] shadow-md py-1 active:bg-slate-50"
          onClick={(e: React.MouseEvent) => {
            sendToWebView({
              id: "road network",
              command: "Select_Road_Network",
            });

            setPm({
              roadNetwork: [
                [
                  [0, 0, 0],
                  [1, 2, 0],
                  [2, 5, 0],
                ],
                [
                  [4, 7, 0],
                  [10, 34, 2],
                ],
              ],
            });
          }}
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
