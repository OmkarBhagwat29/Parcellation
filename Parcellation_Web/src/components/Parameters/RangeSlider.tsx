import { useState, useCallback, useRef, useEffect } from "react";
import { debounce } from "lodash";
import { sendToWebView } from "../../app/webview/webview";

const RangeSlider = ({
  id,
  command,
  label,
  range = { min: 0, max: 100 },
  initialValues = { start: 15, end: 60 },
}) => {
  const [values, setValues] = useState(initialValues);
  const [isDragging, setIsDragging] = useState(null);
  const [showTooltip, setShowTooltip] = useState({ start: false, end: false });

  const sliderRef = useRef(null);

  const updateValue = useCallback(
    debounce((newValues) => {
      setValues(newValues);
      sendToWebView({
        id,
        command,
        payload: { Min: newValues.start, Max: newValues.end },
      });
    }, 50),
    []
  );

  const calculatePosition = (clientX) => {
    const sliderRect = sliderRef.current.getBoundingClientRect();
    const position = ((clientX - sliderRect.left) / sliderRect.width) * 100;
    return Math.min(Math.max(position, 0), 100);
  };

  const handleMouseDown = (handle) => (e) => {
    setIsDragging(handle);
    e.preventDefault();
  };

  const handleMouseMove = useCallback(
    (e) => {
      if (!isDragging) return;
      const newPosition = calculatePosition(e.clientX);
      const newValues = { ...values };

      if (isDragging === "start") {
        newValues.start = Math.min(newPosition, values.end - 1);
      } else {
        newValues.end = Math.max(newPosition, values.start + 1);
      }

      updateValue(newValues);
    },
    [isDragging, values, updateValue]
  );

  const handleMouseUp = () => setIsDragging(null);

  useEffect(() => {
    if (isDragging) {
      document.addEventListener("mousemove", handleMouseMove);
      document.addEventListener("mouseup", handleMouseUp);
    }
    return () => {
      document.removeEventListener("mousemove", handleMouseMove);
      document.removeEventListener("mouseup", handleMouseUp);
    };
  }, [isDragging, handleMouseMove]);

  return (
    <div className="w-full max-w-2xl mx-auto px-4">
      <div className="relative h-16" ref={sliderRef}>
        <div className="absolute h-1 w-full bg-gray-200 rounded-full top-1/2 -translate-y-1/2">
          <div
            className="absolute h-full bg-gray-900 rounded-full"
            style={{
              left: `${values.start}%`,
              width: `${values.end - values.start}%`,
            }}
          />
        </div>

        {["start", "end"].map((handle) => (
          <button
            key={handle}
            className={`absolute w-4 h-4 top-1/2 -ml-3 -translate-y-1/2 bg-gray-700 rounded-full shadow-lg focus:ring-2 focus:ring-gray-400 focus:ring-offset-2 ${
              isDragging === handle ? "scale-110" : ""
            } hover:scale-110 transition-transform`}
            style={{ left: `${values[handle]}%` }}
            onMouseDown={handleMouseDown(handle)}
            onMouseEnter={() =>
              setShowTooltip((prev) => ({ ...prev, [handle]: true }))
            }
            onMouseLeave={() =>
              setShowTooltip((prev) => ({ ...prev, [handle]: false }))
            }
            role="slider"
            aria-valuenow={values[handle]}
            aria-valuemin={handle === "start" ? range.min : values.start}
            aria-valuemax={handle === "end" ? range.max : values.end}
            tabIndex={0}
          >
            {showTooltip[handle] && (
              <div className="absolute -top-6 left-1/2 -translate-x-1/2 bg-gray-900 text-white px-0.5 rounded text-sm">
                {Math.round(values[handle])}
              </div>
            )}
          </button>
        ))}
      </div>
      <div className="text-center text-sm -mt-4">
        {label}: {Math.round(values.start)} mts - {Math.round(values.end)} mts
      </div>
    </div>
  );
};

const ParcelSliders = () => {
  return (
    <div>
      <RangeSlider
        id="parcel-depth"
        command="PARCEL_DEPTH_RANGE"
        label="Parcel Depth Range"
        initialValues={{ start: 15, end: 60 }}
      />
      <RangeSlider
        id="parcel-width"
        command="PARCEL_WIDTH_RANGE"
        label="Parcel Width Range"
        initialValues={{ start: 10, end: 50 }}
      />
    </div>
  );
};

export default ParcelSliders;
