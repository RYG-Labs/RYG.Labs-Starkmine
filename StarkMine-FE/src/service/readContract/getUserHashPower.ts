import { ErrorLevelEnum, MessageBase, MessageEnum, StatusEnum } from "@/type/common";
import { rewardDistributorContract } from ".";

const getUserHashPower = async (address: string): Promise<MessageBase> => {
    const userHashPower = await rewardDistributorContract.user_hash_power(address)
    console.log("🚀 ~ getUserHashPower ~ userHashPower:", Number(BigInt(userHashPower)) / 1e12)
    return {
        status: StatusEnum.SUCCESS,
        message: MessageEnum.SUCCESS,
        level: ErrorLevelEnum.INFOR,
        data: Number(BigInt(userHashPower)) / 1e12,
    }
};

export default getUserHashPower;