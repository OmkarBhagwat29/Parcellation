import React, { FC, useCallback, useEffect, useRef, useState } from "react";
import { getDecimalCount, roundUp } from "@/math/math";
import { sendToWebView, WebViewInputProps } from "@/webview/webview";
import { debounce } from "lodash";

interface SliderTheme {
  track: string;
  fill: string;
  thumb: string;
  valueLabel: string;
}

interface SliderProps {
  webViewProps?: WebViewInputProps;
  value: number;
  start: number;
  step: number;
  end: number;
  units?: string;
  classname?: string;
  theme: SliderTheme;
  onChange?: (value: number) => void;
}

const Slider: FC<SliderProps> = ({
  webViewProps,
  value,
  start,
  step,
  end,
  units,
  theme,
  onChange,
  classname = "",
}) => {
  const [currentValue, setCurrentValue] = useState(value);
  const [editing, setEditing] = useState(false);
  const [inputValue, setInputValue] = useState(value.toString());
  const trackRef = useRef<HTMLDivElement>(null);

  const decimalCount = getDecimalCount(step);

  const debouncedValueChange = useCallback(
    debounce((value: number) => {
      if (webViewProps) {
        sendToWebView({
          id: webViewProps.id,
          command: webViewProps.command,
          payload: { value },
        });
      }
    }, 100),
    [webViewProps]
  );

  const updateValue = (val: number) => {
    const clamped = Math.min(end, Math.max(start, val));
    setCurrentValue(clamped);
    onChange?.(clamped);
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
        className={`relative h-1 ${theme.track} rounded-full cursor-pointer ${classname}`}
        onMouseDown={startDrag}
        onTouchStart={startDrag}
      >
        <div
          className={`absolute h-1 ${theme.fill} rounded-full`}
          style={{ width: `${percent}%` }}
        />
        <div
          className={`absolute top-1/2 w-3 h-3 ${theme.thumb} rounded-full transform -translate-y-1/2 -translate-x-1/2`}
          style={{ left: `${percent}%` }}
        />
        <div
          className={`absolute text-xs ${theme.valueLabel} px-1 py-0.5 rounded`}
          style={{
            left: `${percent}%`,
            top: "-28px",
            minWidth: "max-content",
            transform:
              percent < 10
                ? "translateX(0%)"
                : percent > 90
                ? "translateX(-100%)"
                : "translateX(-50%)",
          }}
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
              style={{ appearance: "textfield" }}
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
