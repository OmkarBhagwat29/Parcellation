export const roundUp = (value, decimals) => {
  const factor = Math.pow(10, decimals);
  return Math.ceil(value * factor) / factor;
};

export const getDecimalCount = (value) => {
  const str = value.toString();
  if (str.includes("e-")) {
    // Handle scientific notation like 1e-7
    const [base, exponent] = str.split("e-");
    return parseInt(exponent, 10);
  }
  const decimalPart = str.split(".")[1];
  return decimalPart ? decimalPart.length : 0;
};
