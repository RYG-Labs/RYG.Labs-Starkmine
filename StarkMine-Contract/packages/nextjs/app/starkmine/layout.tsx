"use client";

import React from "react";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { useAccount } from "~~/hooks/useAccount";
import { Address } from "~~/components/scaffold-stark/Address";
import { Balance } from "~~/components/scaffold-stark/Balance";
import { CustomConnectButton } from "~~/components/scaffold-stark/CustomConnectButton";
import { HeaderMenuLinks } from "~~/components/Header";

interface Tab {
    id: string;
    name: string;
    icon: string;
    href: string;
}

const tabs: Tab[] = [
    { id: "miners", name: "Miners", icon: "⛏️", href: "/starkmine/miners" },
    { id: "engines", name: "Engines", icon: "🔧", href: "/starkmine/engines" },
    { id: "station", name: "Station", icon: "🏭", href: "/starkmine/station" },
    { id: "merge", name: "Merge", icon: "🔄", href: "/starkmine/merge" },
    { id: "rewards", name: "Rewards", icon: "💰", href: "/starkmine/rewards" },
];

export default function StarkMineLayout({
    children,
}: {
    children: React.ReactNode;
}) {
    const { address, isConnected } = useAccount();
    const pathname = usePathname();

    // Determine active tab based on current pathname
    const getActiveTab = () => {
        const currentTab = tabs.find(tab => tab.href === pathname);
        return currentTab?.id || "overview";
    };

    return (
        <div className="min-h-screen bg-gradient-to-br from-base-200 to-base-300">
            {/* Navigation */}
            <nav role="navigation" className="container mx-auto px-4 py-4">
                <div className="tabs tabs-boxed bg-base-100 shadow-lg">
                    {tabs.map((tab) => (
                        <Link
                            key={tab.id}
                            href={tab.href}
                            className={`tab tab-lg ${getActiveTab() === tab.id ? "tab-active" : ""
                                } gap-2`}
                        >
                            <span className="text-lg">{tab.icon}</span>
                            {tab.name}
                        </Link>
                    ))}
                </div>
            </nav>

            {/* Main Content */}
            <div className="container mx-auto px-4 pb-8">
                <div className="bg-base-100 rounded-2xl shadow-xl p-6">
                    {children}
                </div>
            </div>
        </div>
    );
} 