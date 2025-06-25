import { useEffect, useState } from "react";

export const useWindowBlur = () => {
  const [isWindowBlurred, setIsWindowBlurred] = useState(false);

  useEffect(() => {
    const handleVisibilityChange = () => {
      setIsWindowBlurred(document.hidden);
    };

    document.addEventListener("visibilitychange", handleVisibilityChange);

    return () => {
      document.removeEventListener("visibilitychange", handleVisibilityChange);
    };
  }, []);

  return isWindowBlurred;
};
