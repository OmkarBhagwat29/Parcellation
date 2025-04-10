import { useTheme } from "@/context/ThemeContext";
import { themes } from "@/theme";
import { sendToWebView, WebViewInputProps } from "@/webview/webview";
import React, { FC } from "react";

interface ButtonProps {
  webViewProps?: WebViewInputProps;
  name: string;
  width?: string; // Accept dynamic width as a prop
  maxWidth?: string; // Accept dynamic maxWidth as a prop
  className?: string; // Accept additional custom className
}

const Button: FC<ButtonProps> = ({
  webViewProps,
  name,
  width = "w-40", // Default width
  maxWidth = "max-w-xs", // Default maxWidth
  className = "", // Default to empty string if no className is passed
}) => {
  const { theme } = useTheme();
  const currentTheme = themes[theme]; // Get current theme from context

  const handleOnClick = () => {
    if (webViewProps) {
      sendToWebView({
        id: webViewProps.id,
        command: webViewProps.command,
        payload: undefined,
      });
    }
  };

  return (
    <button
      className={`cursor-pointer ${currentTheme.button} animation-all duration-200 px-2 py-1 rounded-md ${width} break-words ${className}`} // Apply dynamic button styles with custom className
      onClick={handleOnClick}
      style={{ maxWidth }} // Max width passed as a prop
    >
      {name}
    </button>
  );
};

export default Button;
