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

/**
 * Remaps a number from one range to another.
 *
 * @param value - The number to remap
 * @param inMin - Minimum of the input range
 * @param inMax - Maximum of the input range
 * @param outMin - Minimum of the output range
 * @param outMax - Maximum of the output range
 * @returns The remapped value
 */
export const remap = (
  value: number,
  inMin: number,
  inMax: number,
  outMin: number,
  outMax: number
): number => {
  if (inMin === inMax) {
    throw new Error("Input range cannot be zero (inMin === inMax)");
  }

  const clampedValue = Math.min(Math.max(value, inMin), inMax); // optional: clamp input
  const ratio = (clampedValue - inMin) / (inMax - inMin);
  return outMin + ratio * (outMax - outMin);
};
