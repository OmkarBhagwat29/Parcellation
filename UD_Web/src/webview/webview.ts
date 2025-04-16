declare global {
  interface Window {
    chrome: {
      webview: {
        postMessage: (data: any) => void;
        addEventListener: (
          event: string,
          handler: (event: any) => void
        ) => void;
        removeEventListener: (
          event: string,
          handler: (event: any) => void
        ) => void;
      };
    };
  }
}

export const isWebViewMode = (): boolean => {
  if (window.chrome && window.chrome.webview) {
    return true;
  }

  return false;
};

export const sendToWebView = (data: WebViewSendProps) => {
  if (window.chrome && window.chrome.webview) {
    window.chrome.webview.postMessage(data);
  }
};

export type eventType = "info_message" | "pie_chart_data";

export interface WebViewSendProps {
  id: string;
  command: string;
  payload?: object;
}

export interface WebViewInputProps {
  id: string;
  command: string;
}

export interface ReceiveMessageProps {
  eventType: eventType;
  message: object | string;
}

export interface PieChartProps {
  title: string;
  labels: string[];
  datasets: {
    label: string;
    data: number[];
    backgroundColor: string[];
    hoverOffset: number;
  }[];
}
