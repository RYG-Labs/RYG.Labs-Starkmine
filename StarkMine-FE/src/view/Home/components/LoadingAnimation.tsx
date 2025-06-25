"use client";
import * as React from "react";
import { motion } from "framer-motion";
import "./style.css";
import Image from "next/image";
import Icon from "@/assets/sunnyside_icon.svg";

const border = {
  hidden: {
    opacity: 0,
    pathLength: 0,
    fill: "rgba(255, 255, 255, 0)",
  },
  visible: {
    opacity: 1,
    pathLength: 1,
    fill: "rgba(255, 255, 255, 1)",
  },
};
const inside = {
  hidden: {
    opacity: 0,
    pathLength: 0,
  },
  visible: {
    opacity: 1,
    pathLength: 1,
  },
};

export const Loading = ({
  onAnimationComplete,
}: {
  onAnimationComplete: () => void;
}) => {
  return (
    <motion.div>
      <motion.img
        src={Icon.src}
        alt="loading"
        width={319}
        height={425}
        initial={{ y: -10 }}
        animate={{ y: 10 }}
        transition={{
          type: "smooth",
          repeatType: "mirror",
          duration: 2,
          repeat: Infinity,
        }}
        onAnimationComplete={() => {
          onAnimationComplete();
        }}
      />
      {/* <motion.svg
        xmlns="http://www.w3.org/2000/svg"
        viewBox="-10 -10 610 160"
        preserveAspectRatio="xMidYMid meet"
        className="item"
        animate={{ opacity: 1 }}
      >
        <motion.path
          d="M384.711 247.977H596.689V345.133C596.689 350.011 592.734 353.966 587.856 353.966H393.544C388.666 353.966 384.711 350.011 384.711 345.133V247.977Z"
          fill="#FEE109"
          variants={border}
          initial="hidden"
          animate="visible"
          transition={{
            default: { duration: 1, ease: "easeInOut" },
            fill: { duration: 2, ease: [1, 0, 0.5, 1] },
          }}
          //   onAnimationComplete={() => onAnimationComplete()}
          // onAnimationComplete={() => {
          //   onAnimationComplete();
          // }}
        />
      </motion.svg> */}
    </motion.div>
  );
};
