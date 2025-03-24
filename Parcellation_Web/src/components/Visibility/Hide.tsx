import React from "react";
import { sendToWebView } from "../webview";

const Hide = () => {
  return (
    <div className="flex gap-2 cursor-pointer text-sm select-none">
      <input
        type="checkbox"
        id="hide"
        className="cursor-pointer"
        onChange={(e) => {
          sendToWebView({
            id: "hide",
            command: "HIDE",
            payload: { Hide: e.target.checked },
          });
        }}
      />
      <label htmlFor="hide" className="cursor-pointer">
        Hide
      </label>
    </div>
  );
};

export default Hide;
