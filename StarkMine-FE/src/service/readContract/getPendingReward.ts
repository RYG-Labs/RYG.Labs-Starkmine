import { ErrorLevelEnum, MessageBase, MessageEnum, StatusEnum } from "@/type/common";
import { convertWeiToEther } from "@/utils/helper";
import { rewardDistributorContract } from ".";

const getPendingReward = async (address: string): Promise<MessageBase> => {
    try {
        const pendingReward = await rewardDistributorContract.pending_rewards(address);
        console.log("🚀 ~ getPendingReward ~ pendingReward:", pendingReward)

    return {
        status: StatusEnum.SUCCESS,
        message: MessageEnum.SUCCESS,
        level: ErrorLevelEnum.INFOR,
        data: {
            pendingReward: Number(convertWeiToEther(pendingReward.pending_rewards)),
            lastClaimed: Number(pendingReward.last_updated_block),
        }
    }
    } catch (error: any) {
        return {
            status: StatusEnum.ERROR,
            message: error.message,
            level: ErrorLevelEnum.WARNING,
            data: {
                pendingReward: 0,
            },
        }
    }
}

export default getPendingReward;