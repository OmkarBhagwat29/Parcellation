export interface ParcellationParameters {
  parcelPoints: number[][];
  roadNetwork: number[][][];
  majorRoadWidth: number;
  minorRoadWidth: number;
}

export const createDefaultParcellationParams = (): ParcellationParameters => {
  const params: ParcellationParameters = {
    majorRoadWidth: 5,
    minorRoadWidth: 9,
    parcelPoints: [],
    roadNetwork: [],
  };

  return params;
};
