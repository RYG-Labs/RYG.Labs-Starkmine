export const convertWeiToEther = (wei: string) => {
    const divisor = BigInt(10 ** 18);
    const bigIntNum = BigInt(wei);
    const scaledNum = bigIntNum / divisor;
    return `${scaledNum}`;
  }