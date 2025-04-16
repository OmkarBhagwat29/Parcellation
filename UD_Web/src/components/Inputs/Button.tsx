import { sendToWebView, WebViewInputProps } from "@/webview/webview";
import React, { FC } from "react";

interface ButtonProps {
  webViewProps?: WebViewInputProps;
  name: string;
  className?: string; // Accept additional custom className
  onClick?: () => void; // Optional onClick handler
}

const Button: FC<ButtonProps> = ({
  webViewProps,
  name,
  className = "",
  onClick,
}) => {
  const handleOnClick = () => {
    if (webViewProps) {
      sendToWebView({
        id: webViewProps.id,
        command: webViewProps.command,
        payload: undefined,
      });
    }

    if (onClick) {
      onClick();
    }
  };

  return (
    <button
      className={`cursor-pointer animation-all duration-200 rounded-md break-words ${className}`} // Apply dynamic button styles with custom className
      onClick={handleOnClick}
      // Max width passed as a prop
    >
      {name}
    </button>
  );
};

export default Button;
