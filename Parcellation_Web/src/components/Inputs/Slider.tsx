import { useTheme } from "@/context/ThemeContext";
import { getDecimalCount, roundUp } from "@/math/math";
import { themes } from "@/theme";
import { sendToWebView, WebViewInputProps } from "@/webview/webview";
import { debounce } from "lodash";
import React, { FC, useCallback, useEffect, useRef, useState } from "react";

interface SliderProps {
  webViewProps?: WebViewInputProps;
  value: number;
  start: number;
  step: number;
  end: number;
  units?: string;
}

const Slider: FC<SliderProps> = ({
  webViewProps,
  value,
  start,
  step,
  end,
  units,
}) => {
  const { theme } = useTheme();
  const currentTheme = themes[theme];

  const [currentValue, setCurrentValue] = useState(value);
  const [editing, setEditing] = useState(false);
  const [inputValue, setInputValue] = useState(value.toString());
  const trackRef = useRef<HTMLDivElement>(null);

  const [decimalCount, setDecimalCount] = useState(getDecimalCount(step));

  const debouncedValueChange = useCallback(
    debounce((value) => {
      sendToWebView({
        id: webViewProps?.id,
        command: webViewProps?.command,
        payload: { value },
      });
    }, 100),
    [webViewProps]
  );

  const updateValue = (val: number) => {
    const clamped = Math.min(end, Math.max(start, val));
    setCurrentValue(clamped);
    debouncedValueChange(clamped);
  };

  const handleDrag = (clientX: number) => {
    const track = trackRef.current;
    if (!track) return;
    const rect = track.getBoundingClientRect();
    const totalSteps = Math.round((end - start) / step);
    const relativeX = Math.max(0, Math.min(clientX - rect.left, rect.width));
    const stepIndex = Math.round((relativeX / rect.width) * totalSteps);

    const steppedValue = roundUp(start + stepIndex * step, decimalCount);
    updateValue(steppedValue);
  };

  const startDrag = (e: React.MouseEvent | React.TouchEvent) => {
    const moveHandler = (e: MouseEvent | TouchEvent) => {
      const x = "touches" in e ? e.touches[0].clientX : e.clientX;
      handleDrag(x);
    };
    const stopHandler = () => {
      document.removeEventListener("mousemove", moveHandler);
      document.removeEventListener("mouseup", stopHandler);
      document.removeEventListener("touchmove", moveHandler);
      document.removeEventListener("touchend", stopHandler);
    };

    document.addEventListener("mousemove", moveHandler);
    document.addEventListener("mouseup", stopHandler);
    document.addEventListener("touchmove", moveHandler);
    document.addEventListener("touchend", stopHandler);
  };

  const inputRef = useRef<HTMLInputElement>(null);
  useEffect(() => {
    if (editing && inputRef.current) {
      inputRef.current.select();
    }
  }, [editing]);

  const percent = ((currentValue - start) / (end - start)) * 100;

  return (
    <>
      <div
        ref={trackRef}
        className={`relative h-1 ${currentTheme.sliderTrack} rounded-full cursor-pointer`}
        onMouseDown={startDrag}
        onTouchStart={startDrag}
      >
        {/* Fill */}
        <div
          className={`absolute h-1 ${currentTheme.sliderFill} rounded-full`}
          style={{ width: `${percent}%` }}
        />
        {/* Thumb */}
        <div
          className={`absolute top-1/2 w-3 h-3 ${currentTheme.sliderThumb} rounded-full transform -translate-y-1/2 -translate-x-1/2`}
          style={{ left: `${percent}%` }}
        />
        {/* Value display */}
        <div
          className={`absolute text-xs ${currentTheme.sliderValue} px-1 py-0.5 rounded transform -translate-x-1/2`}
          style={{ left: `${percent}%`, top: "-28px", minWidth: "max-content" }}
          onClick={() => {
            setInputValue(currentValue.toString());
            setEditing(true);
          }}
        >
          {editing ? (
            <input
              ref={inputRef}
              type="text"
              value={inputValue}
              onChange={(e) => setInputValue(e.target.value)}
              onBlur={() => {
                const num = parseFloat(inputValue);
                if (!isNaN(num)) updateValue(num);
                setEditing(false);
              }}
              onKeyDown={(e) => {
                if (e.key === "Enter") {
                  const num = parseFloat(inputValue);
                  if (!isNaN(num)) updateValue(num);
                  setEditing(false);
                } else if (e.key === "Escape") {
                  setEditing(false);
                }
              }}
              className="bg-gray-700 text-white text-xs w-12 outline-none text-center"
              style={{
                appearance: "textfield",
              }}
            />
          ) : (
            `${currentValue} ${units ?? ""}`
          )}
        </div>
      </div>

      <div className="flex justify-between text-sm text-black mt-1">
        <span>
          {start} {units}
        </span>
        <span>
          {end} {units}
        </span>
      </div>
    </>
  );
};

export default Slider;
