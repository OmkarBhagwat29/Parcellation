import { useEffect, useState } from "react";

interface ResponseFromAiProps {
  loading: boolean;
  setLoading: (loading: boolean) => void;
}

const ResponseFromAi = ({ loading, setLoading }: ResponseFromAiProps) => {
  const [response, setResponse] = useState(""); // Stores full AI response
  const [displayText, setDisplayText] = useState(""); // Simulated typing text
  const [index, setIndex] = useState(0); // Tracks current index for typing effect

  useEffect(() => {
    if (!window.chrome || !window.chrome.webview) {
      return;
    }

    const messageReceived = (event: MessageEvent) => {
      const data = event.data;

      try {
        const parsedData = typeof data === "string" ? JSON.parse(data) : data;

        if (parsedData.eventType === "ai_response") {
          setLoading(false);
          setResponse(parsedData.message); // Store full response
          setDisplayText(""); // Reset display text for new message
          setIndex(0); // Reset typing index
        }
      } catch (error) {
        console.error("Invalid message received:", error);
      }
    };

    window.chrome.webview.addEventListener("message", messageReceived);

    return () => {
      window.chrome.webview.removeEventListener("message", messageReceived);
    };
  }, []);

  useEffect(() => {
    if (index < response.length) {
      const typingTimeout = setTimeout(() => {
        setDisplayText((prev) => prev + response[index]); // Add next character
        setIndex((prev) => prev + 1);
      }, 10); // Adjust typing speed here (50ms per character)

      return () => clearTimeout(typingTimeout);
    }
  }, [index, response]);

  return (
    <div className="w-full h-full p-2 rounded-md bg-slate-50 border border-gray-500">
      <div
        className="text-sm overflow-y-auto select-text h-full"
        style={{ whiteSpace: "pre-wrap" }}
      >
        {loading ? (
          <div className="flex justify-center items-center h-full">
            <span>Thinking...</span>
          </div>
        ) : (
          displayText
        )}
      </div>
    </div>
  );
};

export default ResponseFromAi;
