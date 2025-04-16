import {
  ShapeDiverResponseDto,
  ShapeDiverSdk,
} from "@shapediver/sdk.geometry-api-sdk-v2";
import { Object3D } from "three";

export interface Model_Name_Paths {
  name: string;
  paths: string[];
}

export interface SDSessionManager {
  sdk: ShapeDiverSdk | undefined;
  defaultState: ShapeDiverResponseDto | undefined;
  newState: ShapeDiverResponseDto | undefined;
  modelData: Model_Name_Paths[];
  modelIds: Object3D[];
}

export const createSessionDefaults = (): SDSessionManager => {
  return {
    sdk: undefined,
    defaultState: undefined,
    newState: undefined,
    modelData: [],
    modelIds: [],
  };
};
