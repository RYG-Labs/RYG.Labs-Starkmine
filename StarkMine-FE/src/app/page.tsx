"use client";
// import { UnityCanvas } from "@/view/Home/components/UnityCanvas";
import dynamic from "next/dynamic";

const UnityCanvas = dynamic(
  () =>
    import("@/view/Home/components/UnityCanvas").then((mod) => mod.UnityCanvas),
  {
    ssr: false,
  }
);

export default function Home() {
  return <UnityCanvas />;
}
