"use client";

import { themes } from "@/theme";
import React, { createContext, useContext, useState, ReactNode } from "react";

// Define a type for the theme context
interface ThemeContextType {
  theme: keyof typeof themes;
  setTheme: (theme: keyof typeof themes) => void;
}

// Create a context with a default theme (let's assume 'warm')
const ThemeContext = createContext<ThemeContextType>({
  theme: "warm",
  setTheme: () => {},
});

// Theme provider component
export const ThemeProvider: React.FC<{ children: ReactNode }> = ({
  children,
}) => {
  const [theme, setTheme] = useState<keyof typeof themes>("warm");

  return (
    <ThemeContext.Provider value={{ theme, setTheme }}>
      {children}
    </ThemeContext.Provider>
  );
};

// Custom hook to use the theme context
export const useTheme = () => useContext(ThemeContext);
