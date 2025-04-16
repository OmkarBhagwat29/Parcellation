import React, { useEffect } from "react";
import { useMap } from "../MapProvider";
import { sendToWebView } from "@/webview/webview";

const useAppSize = () => {
  const { setAppDimensions } = useMap();

  useEffect(() => {
    if (!window.chrome || !window.chrome.webview) {
      // console.warn("WebView2 is not available.");
      return;
    }
    console.log("MapLegends mounted ************");
    const messageReceived = (event: any) => {
      const data = event.data;
      if (data.eventType === "app_resized") {
        const { width, height } = data.payload;

        console.log("app resized received:", width, height);

        setAppDimensions({ width, height });
        // setMapDimensions({ width, height });
        setTimeout(() => {
          sendToWebView({
            id: "mapGeometry",
            command: "RESIZE",
            payload: null,
          });
        }, 700);
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

export default useAppSize;
