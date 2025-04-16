"use client";

import React from "react";
import Parcellation from "./Parcellation";

const Main = ({ isWebView }) => {
  return (
    <>
      <Parcellation isWebView={isWebView} />
    </>
  );
};

export default Main;
