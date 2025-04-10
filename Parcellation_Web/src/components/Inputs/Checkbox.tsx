import { sendToWebView, WebViewInputProps } from "@/webview/webview";
import React, { FC, useState } from "react";

interface CheckboxProps {
  webViewProps?: WebViewInputProps;
  lableName: string;
  defaultValue: boolean;
}

const Checkbox: FC<CheckboxProps> = ({
  lableName,
  defaultValue,
  webViewProps,
}) => {
  const [isChecked, setIsChecked] = useState(defaultValue);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const newValue = e.target.checked;
    setIsChecked(newValue);

    if (webViewProps) {
      sendToWebView({
        id: webViewProps.id,
        command: webViewProps.command,
        payload: { value: newValue },
      });

      console.log({ value: newValue });
    }
  };

  return (
    <>
      <input
        type="checkbox"
        id={lableName}
        className="cursor-pointer"
        onChange={(e) => {
          handleChange(e);
        }}
        checked={isChecked}
      />

      <label htmlFor={lableName} className="cursor-pointer">
        {lableName}
      </label>
    </>
  );
};

export default Checkbox;
