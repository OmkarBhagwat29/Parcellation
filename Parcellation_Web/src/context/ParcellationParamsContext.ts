import {
  createDefaultParcellationParams,
  ParcellationParameters,
} from "./ParcellationParameters";
import { createContext, useContext } from "react";

interface ParamsProps {
  pm: ParcellationParameters;
  setPm: (update: Partial<ParcellationParameters>) => void; // ✅ Allow partial updates
}

export const ParamsContext = createContext<ParamsProps>({
  pm: createDefaultParcellationParams(),
  setPm: () => {},
});

export const useParamsContext = () => {
  return useContext(ParamsContext);
};
