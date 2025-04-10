import { useTheme } from "@/context/ThemeContext";
import { themes } from "@/theme";
import { sendToWebView, WebViewInputProps } from "@/webview/webview";
import React, { FC, useState } from "react";

interface SlideableToggleProps {
  webViewProps?: WebViewInputProps;
  currentValue?: boolean; // Optional prop to set the initial value of the toggle
  className?: string; // Accept additional custom className
}

const SlideableToggle: FC<SlideableToggleProps> = ({
  webViewProps,
  currentValue = false, // Default to false if not provided
  className,
}) => {
  const { theme } = useTheme(); // Access the current theme

  const currentTheme = themes[theme]; // Get current theme from context

  const [isToggled, setIsToggled] = useState(currentValue);

  const handleToggle = () => {
    setIsToggled(!isToggled);

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
      className={`flex items-center p-1 cursor-pointer rounded-full transition-colors duration-300 ${
        isToggled ? currentTheme.toggleActive : currentTheme.toggleInactive
      } ${className} relative`} // Apply dynamic background color based on toggle state
      onClick={handleToggle}
      style={{ width: "100%", height: "auto", aspectRatio: "2/1" }} // Allow for dynamic width & height
    >
      <div
        className={`w-1/2 h-full bg-white rounded-full shadow-lg transform transition-transform duration-300 ${
          isToggled ? "translate-x-full" : "translate-x-0"
        } ${currentTheme.toggleThumb}`} // Apply dynamic thumb color and ensure thumb is on the correct side
        style={{ transition: "transform 0.3s ease-in-out" }} // Smooth thumb transition
      ></div>
    </div>
  );
};

export default SlideableToggle;
