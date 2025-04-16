import { useState, useEffect } from "react";
import { useMap } from "./MapProvider";
import Button from "../Inputs/Button";
import { useTheme } from "@/context/ThemeContext";
import { themes } from "@/theme";

const MapSearch = () => {
  const [input, setInput] = useState("");
  const [suggestions, setSuggestions] = useState<any[]>([]);
  const { setCoordinates } = useMap(); // Hook to set coordinates
  const { theme } = useTheme();
  const currentTheme = themes[theme];
  const [selected, setSelected] = useState(false);

  // Fetch city suggestions dynamically as the user types
  useEffect(() => {
    if (!input || selected) {
      setSelected(false); // Reset selected state when input changes
      setSuggestions([]);
      return;
    }

    const fetchSuggestions = async () => {
      console.log("Fetching suggestions for:", input); // Debugging line

      const response = await fetch(
        `https://nominatim.openstreetmap.org/search?format=json&q=${input}&addressdetails=1`
      );
      const results = await response.json();
      setSuggestions(results);
    };

    fetchSuggestions();
  }, [input]);

  // Handle city search
  const handleSearch = async () => {
    if (!input) return;

    const response = await fetch(
      `https://nominatim.openstreetmap.org/search?format=json&q=${input}`
    );
    const results = await response.json();

    if (results?.[0]) {
      setCoordinates({
        lat: parseFloat(results[0].lat),
        lng: parseFloat(results[0].lon),
      });

      setSuggestions([]); // Clear suggestions after search
      setSelected(false);
    } else {
      alert("City not found");
    }
  };

  // Select a suggestion and update map coordinates, also set the input
  const handleSelectSuggestion = (name: string, lat: string, lon: string) => {
    setSelected(true); // Set selected to true when a suggestion is clicked
    setInput(name); // Set the city name in the input

    setCoordinates({
      lat: parseFloat(lat),
      lng: parseFloat(lon),
    });
    setSuggestions(() => []); // Clear all suggestions after selection
    console.log(suggestions); // Debugging line
  };

  return (
    <div className="relative flex flex-col w-full gap-2">
      {/* Search input */}
      <div className="relative flex w-full">
        <input
          type="text"
          placeholder="Search city"
          value={input}
          onChange={(e) => setInput(e.target.value)}
          className={`${currentTheme.input} w-full border rounded-l-full px-3 py-2 outline-none`}
        />

        <Button
          name="Search"
          className={`${currentTheme.button} px-4 py-2 rounded-r-full`}
          onClick={handleSearch}
        />
      </div>

      {/* Suggestions list */}
      {suggestions.length > 0 && (
        <ul
          className={`absolute left-0 w-full mt-12 max-h-32 overflow-auto border ${currentTheme.collapseContainer} ${currentTheme.background} z-10 rounded-md shadow-lg`}
        >
          {suggestions.map((suggestion: any, index: number) => (
            <li
              key={index}
              className={`px-4 py-2 cursor-pointer ${currentTheme.dropdownHover} text-sm ${currentTheme.text}`}
              onClick={() =>
                handleSelectSuggestion(
                  suggestion.display_name,
                  suggestion.lat,
                  suggestion.lon
                )
              }
            >
              {suggestion.display_name}
            </li>
          ))}
        </ul>
      )}
    </div>
  );
};

export default MapSearch;
