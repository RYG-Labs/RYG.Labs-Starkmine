import type { Metadata } from "next";
import localFont from "next/font/local";
import "./globals.css";
import logo_starkmine from "@/assets/StarkMine-logo.svg";
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
  title: "StarkMine",
  icons: [logo_starkmine.src],
  description:
    "StarkMine is a decentralized mining game on Starknet where players manage NFT miners, upgrade with core engines, and earn MINE tokens through a proof-of-work simulation. Merge miners for higher tiers, boost efficiency with stations, and strategize around monthly reward halvings in a dynamic, player-driven economy.",
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
