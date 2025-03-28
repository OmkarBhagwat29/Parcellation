import { ReceiveMessageProps } from "@/app/webview/webview";
import React, { useEffect, useState } from "react";

const InfoDisplay = () => {
  const [info, setInfo] = useState<string[]>([]);

  useEffect(() => {
    if (!window.chrome || !window.chrome.webview) {
      // console.warn("WebView2 is not available.");
      return;
    }

    // Event listener function
    const messageReceived = (event: any) => {
      const data = event.data as ReceiveMessageProps;
      if (data.eventType === "info_message") {
        const messageData = data.message as string;

        console.log(messageData);
        setInfo((prev) => [messageData]);
      }
    };

    // Add event listener
    window.chrome.webview.addEventListener("message", messageReceived);

    // Cleanup on unmount
    return () => {
      if (window.chrome && window.chrome.webview) {
        window.chrome.webview.removeEventListener("message", messageReceived);
      }
    };
  }, []);

  return (
    <div>
      <label>Info</label>
      <div className="overflow-hidden">
        <div className="h-20 flex flex-col bg-slate-300 pb-5 rounded-lg shadow-lg p-2">
          {info.map((msg, index) => (
            <div key={index} className="text-sm whitespace-pre-line gap">
              {msg}
            </div>
          ))}
        </div>
      </div>
    </div>
  );
};

export default InfoDisplay;
