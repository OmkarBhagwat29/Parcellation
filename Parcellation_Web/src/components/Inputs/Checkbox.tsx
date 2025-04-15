import { sendToWebView, WebViewInputProps } from "@/webview/webview";
import React from "react";

type Shape = "square" | "round";

type CheckboxProps = {
  checked: boolean;
  onChange: (checked: boolean) => void;
  label?: string;
  shape?: Shape;

  // Optional style overrides
  toggleActiveClass?: string;
  toggleInactiveClass?: string;
  toggleThumbClass?: string;
  labelClass?: string;
  webViewProps?: WebViewInputProps;
};

const Checkbox: React.FC<CheckboxProps> = ({
  checked,
  onChange,
  label,
  shape = "square",
  toggleActiveClass = "bg-gray-500",
  toggleInactiveClass = "bg-gray-200",
  toggleThumbClass = "bg-white",
  labelClass = "text-gray-900",
  webViewProps,
}) => {
  const shapeClass = shape === "round" ? "rounded-full" : "rounded-sm";

  return (
    <label className="flex items-center gap-2 cursor-pointer select-none">
      <div className="relative">
        <input
          type="checkbox"
          checked={checked}
          onChange={(e) => {
            onChange(e.target.checked);

            if (webViewProps) {
              sendToWebView({
                id: webViewProps.id,
                command: webViewProps.command,
                payload: { value: e.target.checked },
              });
            }
          }}
          className="sr-only"
        />
        <div
          className={`w-5 h-5 border-2 transition-all duration-200 border-gray-400 ${shapeClass} ${
            checked ? toggleActiveClass : toggleInactiveClass
          }`}
        >
          {checked && (
            <div
              className={`absolute top-1 left-1 w-3 h-3 ${shapeClass} ${toggleThumbClass}`}
            />
          )}
        </div>
      </div>
      {label && <span className={labelClass}>{label}</span>}
    </label>
  );
};

export default Checkbox;
