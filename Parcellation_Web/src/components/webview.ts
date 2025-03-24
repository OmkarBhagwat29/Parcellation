export const sendToWebView = (data: WebViewSendProps) => {
  console.log(data);
  if (window.chrome && window.chrome.webview) {
    window.chrome.webview.postMessage(data);
  }
};

export interface WebViewSendProps {
  id: string;
  command: string;
  payload?: object;
}
