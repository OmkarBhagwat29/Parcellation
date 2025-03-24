import type { Metadata } from "next";
import "./globals.css";

export const metadata: Metadata = {
  title: "Hello World App",
  description: "A simple Hello World application with Tailwind CSS",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
      <body className="antialiased">{children}</body>
    </html>
  );
}
