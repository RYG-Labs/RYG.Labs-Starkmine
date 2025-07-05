import { ErrorLevelEnum, MessageBase, MessageEnum, SECOND_PER_BLOCK, StatusEnum } from "@/type/common";
import { mineContract } from ".";

const getRemainingBlockForHaving = async (): Promise<MessageBase> => {
    const remainingBlock = await mineContract.remaining_blocks_for_halving();
    console.log("🚀 ~ getRemainingBlockForHaving ~ remainingBlock:", {
        remainingBlock: Number(BigInt(remainingBlock)),
        estimateSecond: Number(BigInt(remainingBlock)) * SECOND_PER_BLOCK,
    })

    return {
        status: StatusEnum.SUCCESS,
        message: MessageEnum.SUCCESS,
        level: ErrorLevelEnum.INFOR,
        data: {
            remainingBlock: Number(BigInt(remainingBlock)),
            estimateSeconds: Number(BigInt(remainingBlock)) * SECOND_PER_BLOCK,
        },
    };
}

export default getRemainingBlockForHaving;