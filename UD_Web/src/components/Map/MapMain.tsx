"use client";
import React from "react";
import MapUi from "./MapUi";
import { ThemeProvider } from "@/context/ThemeContext";

const MapMain = () => {
  return (
    <>
      <ThemeProvider>
        <MapUi />
      </ThemeProvider>
    </>
  );
};

export default MapMain;
