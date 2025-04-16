import type { Metadata } from "next";
import "./globals.css";

export const metadata: Metadata = {
  title: "Parcellation App",
  description: "Parcellation App that rund shape diver api in the background",
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
