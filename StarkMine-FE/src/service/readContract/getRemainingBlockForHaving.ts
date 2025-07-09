import { ErrorLevelEnum, MessageBase, MessageEnum, SECOND_PER_BLOCK, StatusEnum } from "@/type/common";
import { mineContract, provider } from ".";

const getRemainingBlockForHaving = async (): Promise<MessageBase> => {
    const lastHalvingBlock = await mineContract.last_halving_block();
    const halvingInterval = await mineContract.halving_interval();
    const currentBlock = await provider.getBlockNumber();
    const remainingBlocks = (Number(lastHalvingBlock) + Number(halvingInterval)) - Number(currentBlock);

    return {
        status: StatusEnum.SUCCESS,
        message: MessageEnum.SUCCESS,
        level: ErrorLevelEnum.INFOR,
        data: {
            remainingBlock: Number(BigInt(remainingBlocks)),
            estimateSeconds: Number(BigInt(remainingBlocks)) * SECOND_PER_BLOCK,
        },
    };
}

export default getRemainingBlockForHaving;