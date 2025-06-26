import type { NextPage } from "next";
import { getMetadata } from "~~/utils/scaffold-stark/getMetadata";

export const metadata = getMetadata({
    title: "StarkMine",
    description: "Manage your mining operations, rigs, and rewards on Starknet",
});

const StarkMine: NextPage = () => {
    return <div>StarkMine</div>;
};

export default StarkMine; 