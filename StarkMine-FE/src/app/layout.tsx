import type { Metadata } from "next";
import localFont from "next/font/local";
import "./globals.css";
import logo_sunny from "@/assets/logo-sunnyside.svg";
import AppProvider from "@/service/provider/AppProvider";

const geistSans = localFont({
  src: "./fonts/GeistVF.woff",
  variable: "--font-geist-sans",
  weight: "100 900",
});
const geistMono = localFont({
  src: "./fonts/GeistMonoVF.woff",
  variable: "--font-geist-mono",
  weight: "100 900",
});

const galindo = localFont({
  src: "./fonts/Galindo-Regular.ttf",
  variable: "--font-galindo",
  weight: "100 900",
});

export const metadata: Metadata = {
  title: "Sunnyside Acres",
  icons: [logo_sunny.src],
  description:
    "Immerse yourself in a unique pixelated blockchain farm where you can cultivate, harvest, and own digital assets securely and creatively.",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
      <body
        className={`${geistSans.variable} ${geistMono.variable} ${galindo.className} antialiased`}
      >
        <AppProvider>{children}</AppProvider>
      </body>
    </html>
  );
}
