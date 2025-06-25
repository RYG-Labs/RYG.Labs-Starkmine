import { ToastContainer } from "react-toastify";
import StarknetProvider from "./StarknetProvider";
import TanstackProvider from "./TanstackProvider";

const AppProvider = ({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) => {
  return (
    <TanstackProvider>
      <StarknetProvider>
        <ToastContainer />
        {children}
      </StarknetProvider>
    </TanstackProvider>
  );
};

export default AppProvider;
