import { useEffect, useState } from "react";

const ResponseFromAi = ({ loading, setLoading }) => {
  const [response, setResponse] = useState(""); // Stores full AI response
  const [displayText, setDisplayText] = useState(""); // Simulated typing text
  const [index, setIndex] = useState(0); // Tracks current index for typing effect

  useEffect(() => {
    if (!window.chrome || !window.chrome.webview) {
      return;
    }

    const messageReceived = (event) => {
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
    <div className="w-full p-2 rounded-md bg-slate-50 border border-gray-500 flex-[0.75]">
      <div
        className="text-sm overflow-y-auto select-text"
        style={{ whiteSpace: "pre-wrap", maxHeight: "375px" }}
      >
        {loading ? (
          <div className="flex justify-center items-center">
            {/* You can replace this with any loading spinner or animation */}
            <span>Thinking...</span>
          </div>
        ) : (
          displayText // Show the simulated typing text after response
        )}
      </div>
    </div>
  );
};

export default ResponseFromAi;
