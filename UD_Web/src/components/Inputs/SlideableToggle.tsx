import React, { FC, useState } from "react";
import { sendToWebView, WebViewInputProps } from "@/webview/webview";

interface SlideableToggleTheme {
  activeBg: string;
  inactiveBg: string;
  thumb: string;
}

interface SlideableToggleProps {
  webViewProps?: WebViewInputProps;
  currentValue?: boolean;
  className?: string;
  theme: SlideableToggleTheme;
}

const SlideableToggle: FC<SlideableToggleProps> = ({
  webViewProps,
  currentValue = false,
  className = "",
  theme,
}) => {
  const [isToggled, setIsToggled] = useState(currentValue);

  const handleToggle = () => {
    setIsToggled((prev) => !prev);

    if (webViewProps) {
      sendToWebView({
        id: webViewProps.id,
        command: webViewProps.command,
        payload: { value: !isToggled },
      });
    }
  };

  return (
    <div
      className={`flex items-center cursor-pointer rounded-full transition-colors duration-300 relative 
        ${isToggled ? theme.activeBg : theme.inactiveBg} ${className}`}
      onClick={handleToggle}
      style={{ width: "100%", aspectRatio: "2/1" }}
    >
      <div
        className={`w-1/2 h-full bg-white rounded-full shadow-lg transform transition-transform duration-300 
          ${isToggled ? "translate-x-full" : "translate-x-0"} ${theme.thumb}`}
        style={{ transition: "transform 0.3s ease-in-out" }}
      ></div>
    </div>
  );
};

export default SlideableToggle;
