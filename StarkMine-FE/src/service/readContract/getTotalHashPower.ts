import { ErrorLevelEnum, MessageBase, MessageEnum, StatusEnum } from "@/type/common";
import { rewardDistributorContract } from ".";

const getTotalHashPower = async (): Promise<MessageBase> => {
    const totalHashPower = await rewardDistributorContract.total_hash_power()
    console.log("🚀 ~ getTotalHashPower ~ totalHashPower:", Number(BigInt(totalHashPower)) / 1e12)
    return {
        status: StatusEnum.SUCCESS,
        message: MessageEnum.SUCCESS,
        level: ErrorLevelEnum.INFOR,
        data: Number(BigInt(totalHashPower)) / 1e12,
    }
};

export default getTotalHashPower;