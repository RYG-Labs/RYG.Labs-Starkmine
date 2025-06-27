export const convertWeiToEther = (wei: string) => {
    const divisor = BigInt(10 ** 18);
    const bigIntNum = BigInt(wei);
    const scaledNum = bigIntNum / divisor;
    return `${scaledNum}`;
  }

  export const formattedContractAddress = (
    contractAddress: string | undefined
  ) => {
    if (!contractAddress || contractAddress == "") return "";
  
    if (!contractAddress.startsWith("0x")) return contractAddress;
  
    while (contractAddress.trim().length < 66) {
      contractAddress = contractAddress.trim().replace("0x", "0x0");
    }
  
    return contractAddress.toLowerCase().trim();
  };