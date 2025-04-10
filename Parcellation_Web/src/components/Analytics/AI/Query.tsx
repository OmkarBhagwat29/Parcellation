import { RiSendPlane2Fill } from "react-icons/ri";
import { sendToWebView } from "../../../webview/webview";
import { FC, useEffect, useRef, useState } from "react";

interface QueryAiProps {
  loading: boolean;
  handleLoadingState: (state: boolean) => void;
}

const QueryToAi: FC<QueryAiProps> = ({ loading, handleLoadingState }) => {
  const [message, setMessage] = useState(""); // State to store typed message
  const inputRef = useRef<HTMLDivElement>(null); // Ref for contentEditable div

  // Function to handle sending message
  const handleSendMessage = () => {
    if (message.trim() === "") return; // Prevent sending empty messages

    console.log("Sending message:", message);
    handleLoadingState(true); // Set loading to true before sending

    // Send message
    sendToWebView({
      id: "ai_query",
      command: "AI_QUERY",
      payload: { message },
    });
  };

  useEffect(() => {
    // Clear the input text content
    if (!loading) {
      setMessage("");

      if (inputRef.current) {
        inputRef.current.textContent = ""; // Manually clear contentEditable div
      }
    }
  }, [loading]);

  return (
    <div className="mt-3 w-full rounded-md flex-[0.25]">
      {/* Editable Text Area */}
      <div id="content" className="flex flex-col items-end gap-2">
        <div
          ref={inputRef}
          contentEditable="true"
          className="text-sm p-1 h-24 border border-gray-500 w-full bg-slate-50 rounded-md cursor-text overflow-y-auto"
          style={{ whiteSpace: "pre-wrap" }}
          onInput={(e) => setMessage(e.currentTarget.textContent || "")}
        ></div>

        {/* Send Icon */}
        <button
          className="ml-2 p-2 bg-slate-400 text-white rounded-md hover:bg-slate-600 flex items-center justify-center"
          onClick={handleSendMessage} // Send message on click
          disabled={loading}
        >
          <RiSendPlane2Fill size={20} />
        </button>
      </div>
    </div>
  );
};

export default QueryToAi;
