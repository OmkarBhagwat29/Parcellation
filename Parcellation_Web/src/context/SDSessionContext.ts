import { createSessionDefaults, SDSessionManager } from "./SDSessionManager";
import { createContext, useContext } from "react";

interface SDSessionsProps {
  sd: SDSessionManager;
  setSd: (update: Partial<SDSessionManager>) => void;
}

export const SDSessionContext = createContext<SDSessionsProps>({
  sd: createSessionDefaults(),
  setSd: () => {},
});

export const useSDSessionContext = () => {
  return useContext(SDSessionContext);
};
