import { ErrorLevelEnum, MessageBase, MessageEnum, StatusEnum } from "@/type/common";
import { provider } from ".";

const getCurrentBlock = async (): Promise<MessageBase> => {
    const block = await provider.getBlockNumber();
    return {
        status: StatusEnum.SUCCESS,
        message: MessageEnum.SUCCESS,
        level: ErrorLevelEnum.INFOR,
        data: {
            currentBlock: Number(block),
        },
    }
}

export default getCurrentBlock;