"use client";
import React from "react";
import FloodingToolUi from "./FloodingToolUi";
import { ThemeProvider } from "@/context/ThemeContext";

const FloodingMain = () => {
  return (
    <>
      <ThemeProvider>
        <FloodingToolUi />
      </ThemeProvider>
    </>
  );
};

export default FloodingMain;
