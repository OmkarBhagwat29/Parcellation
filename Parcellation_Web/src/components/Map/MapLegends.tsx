import React, { useEffect } from "react";
import { useMap } from "./MapProvider";
import { isWebViewMode, sendToWebView } from "@/webview/webview";

const MapLegends = () => {
  const { setIsExtracting } = useMap();

  useEffect(() => {
    if (!window.chrome || !window.chrome.webview) {
      // console.warn("WebView2 is not available.");
      return;
    }

    const messageReceived = (event: any) => {
      const data = JSON.parse(event.data);

      if (data.eventType === "extraction_completed") {
        setIsExtracting(false);

        if (isWebViewMode()) {
          sendToWebView({
            id: "mapGeometry",
            command: "RESIZE",
            payload: null,
          });
        }
      }
    };

    window.chrome.webview.addEventListener("message", messageReceived);

    return () => {
      if (window.chrome && window.chrome.webview) {
        console.log("MapLegends unmounted ************");
        window.chrome.webview.removeEventListener("message", messageReceived);
      }
    };
  }, []);

  return null;
};

export default MapLegends;
