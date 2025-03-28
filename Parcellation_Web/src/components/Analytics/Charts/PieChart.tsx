import React, { useEffect, useState } from "react";
import { Pie } from "react-chartjs-2";
import { Chart as ChartJS, ArcElement, Tooltip, Legend, Title } from "chart.js";
import { PieChartProps, ReceiveMessageProps } from "@/app/webview/webview";

// Register required components
ChartJS.register(ArcElement, Tooltip, Legend, Title);

const PieChart = () => {
  const [chartData, setChartData] = useState<PieChartProps | undefined>(
    undefined
  );

  useEffect(() => {
    if (!window.chrome || !window.chrome.webview) {
      // console.warn("WebView2 is not available.");
      return;
    }

    // Event listener function
    const messageReceived = (event: any) => {
      const data = event.data as ReceiveMessageProps;
      console.log(data);
      if (data.eventType === "pie_chart_data") {
        const chartData = data.message as PieChartProps;
        setChartData(chartData);
        console.log(chartData);
      }
    };
    // Add event listener
    window.chrome.webview.addEventListener("message", messageReceived);

    // Cleanup on unmount
    return () => {
      if (window.chrome && window.chrome.webview) {
        window.chrome.webview.removeEventListener("message", messageReceived);
      }
    };
  }, []);

  return (
    <>
      {chartData && (
        <>
          <Pie
            data={chartData}
            options={{
              plugins: {
                title: {
                  display: true,
                  text: chartData.title,
                  font: {
                    size: 14,
                  },
                  padding: {
                    top: 2,
                    bottom: 2,
                  },
                },
                legend: {
                  display: true,
                  position: "bottom",
                  labels: {
                    boxWidth: 20,
                    padding: 5,
                    font: {
                      size: 14,
                    },
                  },
                },
              },
            }}
          />
        </>
      )}
    </>
  );
};

export default PieChart;
