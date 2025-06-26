pub mod interfaces {
    pub mod icore_engine;
}

pub mod token {
    pub mod interface;
    pub mod mine;
}

pub mod nft {
    pub mod core_engine;
    pub mod interface;
    pub mod miner_nft;
}

pub mod mining {
    pub mod interface;
    pub mod merge_system;
    pub mod reward_distributor;
    pub mod station_system;
}
