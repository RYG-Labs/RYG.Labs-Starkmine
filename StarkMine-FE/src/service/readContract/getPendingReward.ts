import { ErrorLevelEnum, MessageBase, MessageEnum, StatusEnum } from "@/type/common";
import { getAbi, provider } from ".";
import { contracts } from "@/configs/contracts";
import { Contract } from "starknet";
import { convertWeiToEther } from "@/utils/helper";

const RewardDistributorContract = new Contract(await getAbi(contracts.RewardDistributor), contracts.RewardDistributor, provider);

const getPendingReward = async (address: string): Promise<MessageBase> => {
    try {
        const pendingReward = await RewardDistributorContract.pending_rewards(address);

    return {
        status: StatusEnum.SUCCESS,
        message: MessageEnum.SUCCESS,
        level: ErrorLevelEnum.INFOR,
        data: {
            pendingReward: Number(convertWeiToEther(pendingReward)),
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